using Hospital.Client.Services;
using Hospital.Shared.DTOs.Patients;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Patients;

public partial class PatientViewModal : ComponentBase
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
    public EventCallback<long> OnEditRequested { get; set; }

    protected PatientDetailsDto? Details { get; set; }
    protected bool IsLoading { get; set; }

    private static readonly string[] AvatarColors =
    [
        "linear-gradient(135deg, #0d9488, #06b6d4)",
        "linear-gradient(135deg, #0891b2, #0e7490)",
        "linear-gradient(135deg, #059669, #0d9488)",
        "linear-gradient(135deg, #7c3aed, #6d28d9)",
        "linear-gradient(135deg, #ec4899, #be185d)",
        "linear-gradient(135deg, #e11d48, #9f1239)",
    ];

    protected string GetAvatarColor(string firstName)
    {
        if (string.IsNullOrEmpty(firstName)) return AvatarColors[0];
        var idx = (firstName[0] - 'A' + 26) % AvatarColors.Length;
        return AvatarColors[Math.Abs(idx)];
    }

    protected string GetGenderBadgeClass(string gender)
    {
        return gender?.ToLower() switch
        {
            "male" => "badge-blue",
            "female" => "badge-rose",
            _ => "badge-slate"
        };
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Show && PatientId > 0 && (Details == null || Details.Id != PatientId))
        {
            await LoadDetailsAsync();
        }
    }

    private async Task LoadDetailsAsync()
    {
        IsLoading = true;
        Details = null;
        try
        {
            var result = await PatientService.GetPatientByIdAsync(PatientId);
            if (result.IsSuccess)
            {
                Details = result.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading patient details: {ex.Message}");
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
        if (PatientId > 0)
        {
            await OnEditRequested.InvokeAsync(PatientId);
        }
    }
}
