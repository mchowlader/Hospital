using Hospital.Client.Services;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Departments;

public partial class DepartmentEditModal : ComponentBase
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
    public EventCallback<string> OnSuccess { get; set; }

    protected UpdateDepartmentRequest FormModel { get; set; } = new();
    protected string? ModalErrorMessage { get; set; }
    protected bool IsSaving { get; set; }

    protected override void OnParametersSet()
    {
        if (Show && Department != null && (FormModel == null || FormModel.Id != Department.Id))
        {
            ResetForm();
        }
    }

    private void ResetForm()
    {
        if (Department != null)
        {
            FormModel = new UpdateDepartmentRequest
            {
                Id = Department.Id,
                Name = Department.Name,
                Description = Department.Description
            };
        }
        ModalErrorMessage = null;
        IsSaving = false;
    }

    protected async Task CloseModal()
    {
        await OnClose.InvokeAsync();
    }

    protected async Task SaveDepartment()
    {
        if (Department == null) return;

        IsSaving = true;
        ModalErrorMessage = null;
        try
        {
            var result = await DepartmentService.UpdateDepartmentAsync(Department.Id, FormModel);
            if (result.IsSuccess)
            {
                var deptName = FormModel.Name;
                await OnSuccess.InvokeAsync(deptName);
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to update department.";
            }
        }
        catch (Exception ex)
        {
            ModalErrorMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }
}
