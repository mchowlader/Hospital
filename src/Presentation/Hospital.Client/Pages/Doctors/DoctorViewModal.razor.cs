using Hospital.Client.Services;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Doctors;

public partial class DoctorViewModal : ComponentBase
{
    [Inject]
    private DoctorServiceClient DoctorService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public DoctorDto? Doctor { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<DoctorDto> OnEditRequested { get; set; }

    protected DoctorDetailsDto? Details { get; set; }
    protected bool IsLoading { get; set; }

    private static readonly string[] AvatarColors =
    [
        "linear-gradient(135deg, #0d9488, #06b6d4)",
        "linear-gradient(135deg, #0891b2, #0e7490)",
        "linear-gradient(135deg, #059669, #0d9488)",
        "linear-gradient(135deg, #7c3aed, #6d28d9)",
        "linear-gradient(135deg, #dc2626, #b91c1c)",
        "linear-gradient(135deg, #d97706, #b45309)",
    ];

    protected string GetAvatarColor(string firstName)
    {
        if (string.IsNullOrEmpty(firstName)) return AvatarColors[0];
        var idx = (firstName[0] - 'A' + 26) % AvatarColors.Length;
        return AvatarColors[Math.Abs(idx)];
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Show && Doctor != null && (Details == null || Details.Id != Doctor.Id))
        {
            await LoadDetailsAsync();
        }
    }

    private async Task LoadDetailsAsync()
    {
        if (Doctor == null) return;
        IsLoading = true;
        Details = null;
        try
        {
            var result = await DoctorService.GetDoctorByIdAsync(Doctor.Id);
            if (result.IsSuccess)
            {
                Details = result.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading doctor details: {ex.Message}");
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
        if (Doctor != null)
        {
            await OnEditRequested.InvokeAsync(Doctor);
        }
    }
}
