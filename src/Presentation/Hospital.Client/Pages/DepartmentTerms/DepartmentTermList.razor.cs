using Hospital.Client.Services;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.DepartmentTerms;

public partial class DepartmentTermList : ComponentBase
{
    [Inject] private DepartmentTermServiceClient TermService { get; set; } = default!;
    [Inject] private DepartmentServiceClient DepartmentService { get; set; } = default!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    protected PagedResult<DepartmentTermDto>? Terms { get; set; }
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

    protected DepartmentTermDto? SelectedTermForEdit { get; set; }
    protected DepartmentTermDto? SelectedTermForView { get; set; }

    private CancellationTokenSource? _searchCts;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        await Task.WhenAll(LoadDepartmentsForFilterAsync(), LoadDataAsync());
    }

    private async Task LoadDepartmentsForFilterAsync()
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
            var result = await TermService.GetDepartmentTermsAsync(CurrentPage, PageSize, SearchTerm, deptId);
            if (result.IsSuccess)
                Terms = result.Value;
            else
                ErrorMessage = result.Error?.Description ?? "Failed to load treatment terms.";
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

    protected async Task OnCreateSuccess(string termName)
    {
        ShowCreateModal = false;
        SuccessMessage = $"Treatment term \"{termName}\" created successfully.";
        await LoadDataAsync();
    }

    protected void OpenEditModal(DepartmentTermDto term)
    {
        SelectedTermForEdit = term;
        ShowEditModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseEditModal()
    {
        ShowEditModal = false;
        SelectedTermForEdit = null;
    }

    protected async Task OnEditSuccess(string termName)
    {
        ShowEditModal = false;
        SelectedTermForEdit = null;
        SuccessMessage = $"Treatment term \"{termName}\" updated successfully.";
        await LoadDataAsync();
    }

    protected void OpenViewModal(DepartmentTermDto term)
    {
        SelectedTermForView = term;
        ShowViewModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseViewModal()
    {
        ShowViewModal = false;
        SelectedTermForView = null;
    }

    protected void OnEditRequestedFromView(DepartmentTermDto term)
    {
        ShowViewModal = false;
        SelectedTermForView = null;
        OpenEditModal(term);
    }

    protected async Task ConfirmDelete(long id, string name)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", $"Delete \"{name}\"? This cannot be undone.");
        if (!confirmed) return;

        ErrorMessage = null;
        SuccessMessage = null;
        try
        {
            var result = await TermService.DeleteDepartmentTermAsync(id);
            if (result.IsSuccess)
            {
                SuccessMessage = $"\"{name}\" deleted successfully.";
                await LoadDataAsync();
            }
            else
                ErrorMessage = result.Error?.Description ?? "Failed to delete term.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unexpected error: {ex.Message}";
        }
    }
}
