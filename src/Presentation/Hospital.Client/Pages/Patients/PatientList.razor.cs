using Hospital.Client.Services;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Patients;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Patients;

public partial class PatientList : ComponentBase
{
    [Inject] private PatientServiceClient PatientService { get; set; } = default!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    protected PagedResult<PatientDto>? Patients { get; set; }
    protected bool IsLoading { get; set; } = true;
    protected bool ShowCreateModal { get; set; }
    protected bool ShowEditModal { get; set; }
    protected bool ShowViewModal { get; set; }
    protected string? SuccessMessage { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchTerm { get; set; } = string.Empty;
    protected string FilterGender { get; set; } = string.Empty;
    protected int CurrentPage { get; set; } = 1;
    protected int PageSize { get; set; } = 10;

    protected long SelectedPatientIdForEdit { get; set; }
    protected long SelectedPatientIdForView { get; set; }

    private CancellationTokenSource? _searchCts;

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

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var genderFilter = string.IsNullOrEmpty(FilterGender) ? null : FilterGender;
            var result = await PatientService.GetPatientsAsync(CurrentPage, PageSize, SearchTerm, genderFilter);
            if (result.IsSuccess)
                Patients = result.Value;
            else
                ErrorMessage = result.Error?.Description ?? "Failed to load patients.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task OnFilterChanged()
    {
        CurrentPage = 1;
        await LoadDataAsync();
    }

    protected async Task OnSearchKeyUp()
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(350, _searchCts.Token);
            CurrentPage = 1;
            await LoadDataAsync();
        }
        catch (TaskCanceledException) { }
    }

    protected async Task ChangePage(int page)
    {
        if (page < 1) return;
        CurrentPage = page;
        await LoadDataAsync();
    }

    protected async Task OnPageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var size))
        {
            PageSize = size;
            CurrentPage = 1;
            await LoadDataAsync();
        }
    }


    protected void OpenAddModal()
    {
        ShowCreateModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseCreateModal() => ShowCreateModal = false;

    protected async Task OnCreateSuccess(string name)
    {
        ShowCreateModal = false;
        SuccessMessage = $"Patient \"{name}\" registered successfully.";
        await LoadDataAsync();
    }

    protected void OpenEditModal(long id)
    {
        SelectedPatientIdForEdit = id;
        ShowEditModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseEditModal()
    {
        ShowEditModal = false;
        SelectedPatientIdForEdit = 0;
    }

    protected async Task OnEditSuccess(string name)
    {
        ShowEditModal = false;
        SelectedPatientIdForEdit = 0;
        SuccessMessage = $"Patient \"{name}\" details updated successfully.";
        await LoadDataAsync();
    }

    protected void OpenViewModal(long id)
    {
        SelectedPatientIdForView = id;
        ShowViewModal = true;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    protected void CloseViewModal()
    {
        ShowViewModal = false;
        SelectedPatientIdForView = 0;
    }

    protected void OnEditRequestedFromView(long id)
    {
        ShowViewModal = false;
        SelectedPatientIdForView = 0;
        OpenEditModal(id);
    }

    protected async Task ConfirmDeletePatient(PatientDto patient)
    {
        var name = patient.FirstName + " " + patient.LastName;
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", $"Delete patient record \"{name}\"? This will soft delete their billing and appointment history.");
        if (!confirmed) return;

        ErrorMessage = null;
        SuccessMessage = null;
        try
        {
            var result = await PatientService.DeletePatientAsync(patient.Id);
            if (result.IsSuccess)
            {
                SuccessMessage = $"\"{name}\" record deleted successfully.";
                await LoadDataAsync();
            }
            else
                ErrorMessage = result.Error?.Description ?? "Failed to delete patient.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unexpected error: {ex.Message}";
        }
    }
}
