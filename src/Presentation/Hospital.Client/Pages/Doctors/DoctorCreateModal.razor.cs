using Hospital.Client.Services;
using Hospital.Shared.DTOs.Departments;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Doctors;

public partial class DoctorCreateModal : ComponentBase
{
    [Inject]
    private DoctorServiceClient DoctorService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    protected CreateDoctorRequest FormModel { get; set; } = new();
    protected string? ModalErrorMessage { get; set; }
    protected bool IsSaving { get; set; }

    protected override void OnParametersSet()
    {
        if (Show && FormModel.FirstName == null)
        {
            ResetForm();
        }
    }

    private void ResetForm()
    {
        FormModel = new CreateDoctorRequest { DepartmentId = 0 };
        ModalErrorMessage = null;
        IsSaving = false;
    }

    protected async Task CloseModal()
    {
        ResetForm();
        await OnClose.InvokeAsync();
    }

    protected async Task SaveDoctor()
    {
        IsSaving = true;
        ModalErrorMessage = null;
        try
        {
            var result = await DoctorService.CreateDoctorAsync(FormModel);
            if (result.IsSuccess)
            {
                var docName = $"Dr. {FormModel.FirstName} {FormModel.LastName}";
                ResetForm();
                await OnSuccess.InvokeAsync(docName);
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to add doctor.";
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
