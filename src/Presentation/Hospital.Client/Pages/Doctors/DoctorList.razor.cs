using Hospital.Client.Services;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Doctors;

public partial class DoctorList : ComponentBase
{
    [Inject] private DoctorServiceClient DoctorService { get; set; } = default!;
    [Inject] private DepartmentServiceClient DepartmentService { get; set; } = default!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    protected PagedResult<DoctorDto>? Doctors { get; set; }
    protected List<DepartmentDto> AllDepartments { get; set; } = new();
    protected bool IsLoading { get; set; } = true;
    protected bool ShowCreateModal { get; set; }
    protected bool ShowEditModal { get; set; }
    protected bool ShowViewModal { get; set; }
    protected string? SuccessMessage { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchTerm { get; set; } = string.Empty;
    protected long FilterDepartmentId { get; set; } = 0;
    protected int CurrentPage { get; set; } = 1;
    protected int PageSize { get; set; } = 10;

    protected DoctorDto? SelectedDoctorForEdit { get; set; }
    protected DoctorDto? SelectedDoctorForView { get; set; }
    
    private CancellationTokenSource? _searchCts;

    // Avatar background colors cycling through teal palette
    private static readonly string[] AvatarColors =
    [
        "linear-gradient(135deg, #0d9488, #06b6d4)",
        "linear-gradient(135deg, #0891b2, #0e7490)",
        "linear-gradient(135deg, #059669, #0d9488)",
        "linear-gradient(135deg, #7c3aed, #6d28d9)",
        "linear-gradient(135deg, #dc2626, #b91c1c)",
        "linear-gradient(135deg, #d97706, #b45309)",
    ];

    protected string GetAvatarColor(string firstName)
    {
        if (string.IsNullOrEmpty(firstName)) return AvatarColors[0];
        var idx = (firstName[0] - 'A' + 26) % AvatarColors.Length;
        return AvatarColors[Math.Abs(idx)];
    }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        await Task.WhenAll(LoadDepartmentsAsync(), LoadDataAsync());
    }

    private async Task LoadDepartmentsAsync()
    {
        try
        {
            var result = await DepartmentService.GetDepartmentsAsync(1, 200, null);
            if (result.IsSuccess && result.Value != null)
                AllDepartments = result.Value.Items;
        }
        catch { }
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var deptId = FilterDepartmentId > 0 ? (long?)FilterDepartmentId : null;
            var result = await DoctorService.GetDoctorsAsync(CurrentPage, PageSize, SearchTerm, deptId);
            if (result.IsSuccess)
                Doctors = result.Value;
            else
                ErrorMessage = result.Error?.Description ?? "Failed to load doctors.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task OnFilterChanged()
    {
        CurrentPage = 1;
        await LoadDataAsync();
    }

    protected async Task OnSearchKeyUp()
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(350, _searchCts.Token);
            CurrentPage = 1;
            await LoadDataAsync();
        }
        catch (TaskCanceledException) { }
    }

    protected async Task ChangePage(int page)
    {
        if (page < 1) return;
        CurrentPage = page;
        await LoadDataAsync();
    }

    protected async Task OnPageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var size))
        {
            PageSize = size;
            CurrentPage = 1;
            await LoadDataAsync();
        }
    }


    protected void OpenAddModal()
    {
        ShowCreateModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseCreateModal() => ShowCreateModal = false;

    protected async Task OnCreateSuccess(string docName)
    {
        ShowCreateModal = false;
        SuccessMessage = $"Doctor \"{docName}\" added successfully.";
        await LoadDataAsync();
    }

    protected void OpenEditModal(DoctorDto doc)
    {
        SelectedDoctorForEdit = doc;
        ShowEditModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseEditModal()
    {
        ShowEditModal = false;
        SelectedDoctorForEdit = null;
    }

    protected async Task OnEditSuccess(string docName)
    {
        ShowEditModal = false;
        SelectedDoctorForEdit = null;
        SuccessMessage = $"Doctor \"{docName}\" updated successfully.";
        await LoadDataAsync();
    }

    protected void OpenViewModal(DoctorDto doc)
    {
        SelectedDoctorForView = doc;
        ShowViewModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseViewModal()
    {
        ShowViewModal = false;
        SelectedDoctorForView = null;
    }

    protected void OnEditRequestedFromView(DoctorDto doc)
    {
        ShowViewModal = false;
        SelectedDoctorForView = null;
        OpenEditModal(doc);
    }

    protected Task ConfirmDeleteDoctor(DoctorDto doc)
    {
        var name = "Dr. " + doc.FirstName + " " + doc.LastName;
        return ConfirmDelete(doc.Id, name);
    }

    protected async Task ConfirmDelete(long id, string name)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", $"Delete \"{name}\"? This cannot be undone.");
        if (!confirmed) return;

        ErrorMessage = null;
        SuccessMessage = null;
        try
        {
            var result = await DoctorService.DeleteDoctorAsync(id);
            if (result.IsSuccess)
            {
                SuccessMessage = $"\"{name}\" removed successfully.";
                await LoadDataAsync();
            }
            else
                ErrorMessage = result.Error?.Description ?? "Failed to delete doctor.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unexpected error: {ex.Message}";
        }
    }
}
