using Hospital.Client.Services;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.DepartmentTerms;

public partial class DepartmentTermViewModal : ComponentBase
{
    [Inject]
    private DepartmentTermServiceClient TermService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public DepartmentTermDto? Term { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<DepartmentTermDto> OnEditRequested { get; set; }

    protected DepartmentTermDto? Details { get; set; }
    protected bool IsLoading { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Show && Term != null && (Details == null || Details.Id != Term.Id))
        {
            await LoadDetailsAsync();
        }
    }

    private async Task LoadDetailsAsync()
    {
        if (Term == null) return;
        IsLoading = true;
        Details = null;
        try
        {
            var result = await TermService.GetDepartmentTermByIdAsync(Term.Id);
            if (result.IsSuccess)
            {
                Details = result.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading department term details: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task CloseModal()
    {
        Details = null;
        await OnClose.InvokeAsync();
    }

    protected async Task TriggerEdit()
    {
        if (Term != null)
        {
            await OnEditRequested.InvokeAsync(Term);
        }
    }
}
