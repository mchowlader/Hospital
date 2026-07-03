using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class LandingPageSection : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public string SectionType { get; set; } = "General"; // e.g., Hero, Services, About, DoctorProfile, InfoCard
    public bool IsVisible { get; set; } = true;
}
