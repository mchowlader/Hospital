using Hospital.Client.Services;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Departments;

public partial class DepartmentViewModal : ComponentBase
{
    [Inject]
    private DepartmentServiceClient DepartmentService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public DepartmentDto? Department { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<DepartmentDto> OnEditRequested { get; set; }

    protected DepartmentDetailsDto? Details { get; set; }
    protected bool IsLoading { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Show && Department != null && (Details == null || Details.Id != Department.Id))
        {
            await LoadDetailsAsync();
        }
    }

    private async Task LoadDetailsAsync()
    {
        if (Department == null) return;
        IsLoading = true;
        Details = null;
        try
        {
            var result = await DepartmentService.GetDepartmentByIdAsync(Department.Id);
            if (result.IsSuccess)
            {
                Details = result.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading department details: {ex.Message}");
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
        if (Department != null)
        {
            await OnEditRequested.InvokeAsync(Department);
        }
    }
}
