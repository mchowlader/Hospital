using Hospital.Client.Services;
using Hospital.Shared.DTOs.LandingPage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.CMS;

public partial class ManageLandingPage : ComponentBase
{
    [Inject]
    private LandingPageServiceClient LandingPageServiceClient { get; set; } = default!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    protected List<LandingPageSectionDto>? Sections { get; set; }
    protected bool ShowCreateModal { get; set; }
    protected bool ShowEditModal { get; set; }
    protected LandingPageSectionDto? SelectedSection { get; set; }
    protected string? SuccessMessage { get; set; }
    protected string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadSectionsAsync();
    }

    private async Task LoadSectionsAsync()
    {
        try
        {
            var result = await LandingPageServiceClient.GetLandingPageSectionsAsync(includeHidden: true);
            if (result.IsSuccess)
            {
                Sections = result.Value;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading sections: {ex.Message}";
        }
    }

    protected void OpenAddModal()
    {
        SuccessMessage = null;
        ErrorMessage = null;
        ShowCreateModal = true;
    }

    protected void CloseCreateModal()
    {
        ShowCreateModal = false;
    }

    protected void OpenEditModal(LandingPageSectionDto section)
    {
        SuccessMessage = null;
        ErrorMessage = null;
        SelectedSection = section;
        ShowEditModal = true;
    }

    protected void CloseEditModal()
    {
        ShowEditModal = false;
        SelectedSection = null;
    }

    protected async Task OnSectionSaved()
    {
        ShowCreateModal = false;
        ShowEditModal = false;
        SelectedSection = null;
        SuccessMessage = "Section saved successfully.";
        await LoadSectionsAsync();
    }


    protected async Task ConfirmDelete(long id)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this section?");
        if (confirmed)
        {
            ErrorMessage = null;
            SuccessMessage = null;
            try
            {
                var result = await LandingPageServiceClient.DeleteLandingPageSectionAsync(id);
                if (result.IsSuccess)
                {
                    SuccessMessage = "Section deleted successfully.";
                    await LoadSectionsAsync();
                }
                else
                {
                    ErrorMessage = result.Error?.Description ?? "Failed to delete section.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during delete: {ex.Message}";
            }
        }
    }

    protected string GetExcerpt(string content)
    {
        if (string.IsNullOrEmpty(content)) return "";
        return content.Length > 120 ? content[..120] + "..." : content;
    }
}
// Note: We use raw JS confirm modal for simplicity as it requires no custom UI setup.
