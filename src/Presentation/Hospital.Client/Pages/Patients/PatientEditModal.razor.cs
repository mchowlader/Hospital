using Hospital.Client.Services;
using Hospital.Shared.DTOs.Patients;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Patients;

public partial class PatientEditModal : ComponentBase
{
    [Inject]
    private PatientServiceClient PatientService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public long PatientId { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    protected UpdatePatientRequest FormModel { get; set; } = new();
    protected string? ModalErrorMessage { get; set; }
    protected bool IsLoadingDetails { get; set; }
    protected bool IsSaving { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Show && PatientId > 0 && FormModel.Id != PatientId)
        {
            await LoadPatientDetailsAsync();
        }
    }

    private async Task LoadPatientDetailsAsync()
    {
        IsLoadingDetails = true;
        ModalErrorMessage = null;
        try
        {
            var result = await PatientService.GetPatientByIdAsync(PatientId);
            if (result.IsSuccess && result.Value != null)
            {
                var pt = result.Value;
                FormModel = new UpdatePatientRequest
                {
                    Id = pt.Id,
                    FirstName = pt.FirstName,
                    LastName = pt.LastName,
                    Email = pt.Email,
                    PhoneNumber = pt.PhoneNumber,
                    DateOfBirth = pt.DateOfBirth,
                    Gender = pt.Gender,
                    MedicalHistorySummary = pt.MedicalHistorySummary
                };
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to fetch patient details.";
            }
        }
        catch (Exception ex)
        {
            ModalErrorMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsLoadingDetails = false;
        }
    }

    protected async Task CloseModal()
    {
        FormModel = new UpdatePatientRequest();
        await OnClose.InvokeAsync();
    }

    protected async Task SavePatient()
    {
        IsSaving = true;
        ModalErrorMessage = null;
        try
        {
            var result = await PatientService.UpdatePatientAsync(PatientId, FormModel);
            if (result.IsSuccess)
            {
                var ptName = $"{FormModel.FirstName} {FormModel.LastName}";
                await OnSuccess.InvokeAsync(ptName);
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to update patient.";
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
