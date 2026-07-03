using Hospital.Client.Services;
using Hospital.Shared.DTOs.LandingPage;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Client.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private LandingPageServiceClient LandingPageServiceClient { get; set; } = default!;

    protected List<LandingPageSectionDto>? Sections { get; set; }

    protected List<(string Title, string Desc)> GetTreatmentsList(string content)
    {
        var list = new List<(string Title, string Desc)>();
        if (string.IsNullOrEmpty(content)) return list;
        var blocks = content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var block in blocks)
        {
            var lines = block.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                var title = lines[0].Trim();
                var desc = lines.Length > 1 ? string.Join(" ", lines[1..]) : "";
                list.Add((title, desc));
            }
        }
        return list;
    }

    protected override async Task OnInitializedAsync()

    {
        try
        {
            var result = await LandingPageServiceClient.GetLandingPageSectionsAsync(includeHidden: false);
            if (result.IsSuccess && result.Value != null && result.Value.Count > 0)
            {
                Sections = result.Value;
            }
            else
            {
                Sections = GetDefaultSections();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching landing page sections: {ex.Message}");
            Sections = GetDefaultSections();
        }
    }

    private List<LandingPageSectionDto> GetDefaultSections()
    {
        return new List<LandingPageSectionDto>
        {
            new()
            {
                Id = 1,
                Title = "Farzana's Painless Dental Care",
                Content = "Providing state-of-the-art, stress-free dental treatments with utmost care and precision. Experience modern dentistry designed to keep your smile healthy and beautiful.",
                ImageUrl = "images/dental_banner.png",
                SectionType = "Hero",
                DisplayOrder = 1,
                IsVisible = true
            },
            new()
            {
                Id = 2,
                Title = "Meet Our Senior Consultant",
                Content = "Dr. Abdullah Al Mamun\nBDS (DU), MDS\nPGT (Conservative Dentistry & Endodontics)\nSpecialist in Dental Implants & Root Canal Therapy\nDhaka Central International Medical College & Hospital\nReg No: 11783\nContact: +8801737-591865",
                ImageUrl = "images/doctor_mamun.jpg",
                SectionType = "DoctorProfile",
                DisplayOrder = 2,
                IsVisible = true
            },
            new()
            {
                Id = 3,
                Title = "Our Premium Dental Treatments",
                Content = "Root Canal Therapy\nSave your natural teeth with advanced, painless root canal treatments.\n\nDental Implants\nRestore missing teeth permanently with state-of-the-art dental implants.\n\nCrowns & Bridges\nStrengthen and restore damaged teeth with high-quality porcelain crowns.\n\nScaling & Polishing\nMaintain fresh breath and clean teeth with professional cleaning.",
                ImageUrl = "",
                SectionType = "Treatments",
                DisplayOrder = 3,
                IsVisible = true
            },
            new()
            {
                Id = 4,
                Title = "Advanced Dental Care Technology",
                Content = "Watch how modern painless dentistry helps you maintain perfect oral hygiene without any fear or discomfort.",
                ImageUrl = "https://www.youtube.com/embed/hB2h3V6q-fQ",
                SectionType = "Video",
                DisplayOrder = 4,
                IsVisible = true
            },
            new()
            {
                Id = 5,
                Title = "Excellence in Clinical Hygiene",
                Content = "At Farzana's Dental Care, patient safety and clinical hygiene are our top priorities. We use state-of-the-art sterilisation methods, autoclave systems, and gentle diagnostic tools to ensure a completely safe and pain-free treatment experience.",
                ImageUrl = "",
                SectionType = "About",
                DisplayOrder = 5,
                IsVisible = true
            },
            new()
            {
                Id = 6,
                Title = "Visiting Hours & Location",
                Content = "10:00 AM - 02:00 PM\n04:00 PM - 10:00 PM",
                ImageUrl = "",
                SectionType = "InfoCard",
                DisplayOrder = 6,
                IsVisible = true
            }
        };
    }
}
