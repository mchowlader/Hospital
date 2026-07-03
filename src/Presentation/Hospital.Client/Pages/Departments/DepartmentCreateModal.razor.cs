using Hospital.Client.Services;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Departments;

public partial class DepartmentCreateModal : ComponentBase
{
    [Inject]
    private DepartmentServiceClient DepartmentService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    protected CreateDepartmentRequest FormModel { get; set; } = new();
    protected string? ModalErrorMessage { get; set; }
    protected bool IsSaving { get; set; }

    protected override void OnParametersSet()
    {
        if (Show && FormModel.Name == null)
        {
            ResetForm();
        }
    }

    private void ResetForm()
    {
        FormModel = new CreateDepartmentRequest();
        ModalErrorMessage = null;
        IsSaving = false;
    }

    protected async Task CloseModal()
    {
        ResetForm();
        await OnClose.InvokeAsync();
    }

    protected async Task SaveDepartment()
    {
        IsSaving = true;
        ModalErrorMessage = null;
        try
        {
            var result = await DepartmentService.CreateDepartmentAsync(FormModel);
            if (result.IsSuccess)
            {
                var deptName = FormModel.Name;
                ResetForm();
                await OnSuccess.InvokeAsync(deptName);
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to create department.";
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
