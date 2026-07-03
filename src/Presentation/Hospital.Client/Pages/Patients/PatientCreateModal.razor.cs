using Hospital.Client.Services;
using Hospital.Shared.DTOs.Patients;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Patients;

public partial class PatientCreateModal : ComponentBase
{
    [Inject]
    private PatientServiceClient PatientService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    protected CreatePatientRequest FormModel { get; set; } = new();
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
        FormModel = new CreatePatientRequest
        {
            DateOfBirth = DateTime.UtcNow.AddYears(-25),
            Gender = "Male",
            MedicalHistorySummary = string.Empty
        };
        ModalErrorMessage = null;
        IsSaving = false;
    }

    protected async Task CloseModal()
    {
        ResetForm();
        await OnClose.InvokeAsync();
    }

    protected async Task SavePatient()
    {
        IsSaving = true;
        ModalErrorMessage = null;
        try
        {
            var result = await PatientService.CreatePatientAsync(FormModel);
            if (result.IsSuccess)
            {
                var ptName = $"{FormModel.FirstName} {FormModel.LastName}";
                ResetForm();
                await OnSuccess.InvokeAsync(ptName);
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to register patient.";
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
