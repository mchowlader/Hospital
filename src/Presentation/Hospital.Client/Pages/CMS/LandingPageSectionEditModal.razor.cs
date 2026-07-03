using Hospital.Client.Services;
using Hospital.Shared.DTOs.LandingPage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Hospital.Client.Pages.CMS;

public partial class LandingPageSectionEditModal : ComponentBase
{
    [Inject]
    private LandingPageServiceClient LandingPageService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public LandingPageSectionDto? Section { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback OnSaved { get; set; }

    protected UpdateLandingPageSectionRequest? RequestModel { get; set; }
    protected bool IsSaving { get; set; }
    protected string? ErrorMessage { get; set; }

    protected override void OnParametersSet()
    {
        if (Show && Section != null)
        {
            RequestModel = new UpdateLandingPageSectionRequest
            {
                Id = Section.Id,
                Title = Section.Title,
                Content = Section.Content,
                ImageUrl = Section.ImageUrl,
                DisplayOrder = Section.DisplayOrder,
                SectionType = Section.SectionType,
                IsVisible = Section.IsVisible
            };
            ErrorMessage = null;
        }
    }

    protected async Task Close()
    {
        await OnClose.InvokeAsync();
    }

    protected async Task Save()
    {
        if (RequestModel == null || Section == null) return;

        IsSaving = true;
        ErrorMessage = null;
        try
        {
            var result = await LandingPageService.UpdateLandingPageSectionAsync(Section.Id, RequestModel);
            if (result.IsSuccess)
            {
                await OnSaved.InvokeAsync();
            }
            else
            {
                ErrorMessage = result.Error?.Description ?? "Failed to update section.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    protected string GetImagePlaceholder()
    {
        if (RequestModel == null) return "";
        return RequestModel.SectionType switch
        {
            "Video" => "e.g. https://www.youtube.com/embed/hB2h3V6q-fQ",
            "Hero" => "e.g. images/dental_banner.png",
            "DoctorProfile" => "e.g. images/doctor_mamun.jpg",
            _ => "e.g. images/treatment_scaling.jpg"
        };
    }

    protected string GetImageHelpText()
    {
        if (RequestModel == null) return "";
        return RequestModel.SectionType switch
        {
            "Video" => "Use a YouTube embed URL (with '/embed/').",
            "Hero" => "Default dental banner: images/dental_banner.png",
            "DoctorProfile" => "Doctor portrait photo: images/doctor_mamun.jpg",
            _ => "Path to image or external URL."
        };
    }

    protected async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file != null)
        {
            var isVideo = (!string.IsNullOrEmpty(file.ContentType) && file.ContentType.StartsWith("video/"))
                          || file.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
                          || file.Name.EndsWith(".webm", StringComparison.OrdinalIgnoreCase);

            var maxFileSize = isVideo ? 75 * 1024 * 1024 : 2 * 1024 * 1024; // 75MB for video, 2MB for images
            if (file.Size > maxFileSize)
            {
                ErrorMessage = isVideo ? "Video file size must be under 75MB." : "Image file size must be under 2MB.";
                return;
            }

            try
            {
                var buffer = new byte[file.Size];
                using var stream = file.OpenReadStream(maxFileSize);
                await stream.ReadExactlyAsync(buffer);
                
                var base64 = Convert.ToBase64String(buffer);
                if (RequestModel != null)
                {
                    var contentType = string.IsNullOrEmpty(file.ContentType) ? (isVideo ? "video/mp4" : "image/png") : file.ContentType;
                    RequestModel.ImageUrl = $"data:{contentType};base64,{base64}";
                }
                ErrorMessage = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to read file: {ex.Message}";
            }
        }
    }
}

