using Hospital.Client.Services;
using Hospital.Shared.DTOs.Departments;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Doctors;

public partial class DoctorEditModal : ComponentBase
{
    [Inject]
    private DoctorServiceClient DoctorService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public DoctorDto? Doctor { get; set; }

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    protected UpdateDoctorRequest FormModel { get; set; } = new();
    protected string? ModalErrorMessage { get; set; }
    protected bool IsSaving { get; set; }

    protected override void OnParametersSet()
    {
        if (Show && Doctor != null && (FormModel == null || FormModel.Id != Doctor.Id))
        {
            ResetForm();
        }
    }

    private void ResetForm()
    {
        if (Doctor != null)
        {
            FormModel = new UpdateDoctorRequest
            {
                Id = Doctor.Id,
                FirstName = Doctor.FirstName,
                LastName = Doctor.LastName,
                Email = Doctor.Email,
                PhoneNumber = Doctor.PhoneNumber,
                Specialization = Doctor.Specialization,
                DepartmentId = Doctor.DepartmentId
            };
        }
        ModalErrorMessage = null;
        IsSaving = false;
    }

    protected async Task CloseModal()
    {
        await OnClose.InvokeAsync();
    }

    protected async Task SaveDoctor()
    {
        if (Doctor == null) return;

        IsSaving = true;
        ModalErrorMessage = null;
        try
        {
            var result = await DoctorService.UpdateDoctorAsync(Doctor.Id, FormModel);
            if (result.IsSuccess)
            {
                var docName = $"Dr. {FormModel.FirstName} {FormModel.LastName}";
                await OnSuccess.InvokeAsync(docName);
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to update doctor.";
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
