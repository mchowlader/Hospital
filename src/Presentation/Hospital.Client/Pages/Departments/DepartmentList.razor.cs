using Hospital.Client.Services;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Departments;

public partial class DepartmentList : ComponentBase
{
    [Inject] private DepartmentServiceClient DepartmentService { get; set; } = default!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    protected PagedResult<DepartmentDto>? Departments { get; set; }
    protected bool IsLoading { get; set; } = true;
    protected bool ShowCreateModal { get; set; }
    protected bool ShowEditModal { get; set; }
    protected bool ShowViewModal { get; set; }
    protected string? SuccessMessage { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchTerm { get; set; } = string.Empty;
    protected int CurrentPage { get; set; } = 1;
    protected int PageSize { get; set; } = 10;

    protected DepartmentDto? SelectedDepartmentForEdit { get; set; }
    protected DepartmentDto? SelectedDepartmentForView { get; set; }

    private CancellationTokenSource? _searchCts;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var result = await DepartmentService.GetDepartmentsAsync(CurrentPage, PageSize, SearchTerm);
            if (result.IsSuccess)
                Departments = result.Value;
            else
                ErrorMessage = result.Error?.Description ?? "Failed to load departments.";
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

    protected async Task OnCreateSuccess(string name)
    {
        ShowCreateModal = false;
        SuccessMessage = $"Department \"{name}\" created successfully.";
        await LoadDataAsync();
    }

    protected void OpenEditModal(DepartmentDto dept)
    {
        SelectedDepartmentForEdit = dept;
        ShowEditModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseEditModal()
    {
        ShowEditModal = false;
        SelectedDepartmentForEdit = null;
    }

    protected async Task OnEditSuccess(string name)
    {
        ShowEditModal = false;
        SelectedDepartmentForEdit = null;
        SuccessMessage = $"Department \"{name}\" updated successfully.";
        await LoadDataAsync();
    }

    protected void OpenViewModal(DepartmentDto dept)
    {
        SelectedDepartmentForView = dept;
        ShowViewModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseViewModal()
    {
        ShowViewModal = false;
        SelectedDepartmentForView = null;
    }

    protected void OnEditRequestedFromView(DepartmentDto dept)
    {
        ShowViewModal = false;
        SelectedDepartmentForView = null;
        OpenEditModal(dept);
    }

    protected async Task ConfirmDelete(long id, string name)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", $"Delete \"{name}\"? This action cannot be undone.");
        if (!confirmed) return;

        ErrorMessage = null;
        SuccessMessage = null;
        try
        {
            var result = await DepartmentService.DeleteDepartmentAsync(id);
            if (result.IsSuccess)
            {
                SuccessMessage = $"\"{name}\" deleted successfully.";
                await LoadDataAsync();
            }
            else
                ErrorMessage = result.Error?.Description ?? "Failed to delete department.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unexpected error: {ex.Message}";
        }
    }
}
