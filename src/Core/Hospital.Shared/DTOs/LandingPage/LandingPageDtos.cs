using System.ComponentModel.DataAnnotations;

namespace Hospital.Shared.DTOs.LandingPage;

public class LandingPageSectionDto
{
    public long Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public string SectionType { get; set; } = "General";
    public bool IsVisible { get; set; }
}

public class CreateLandingPageSectionRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = default!;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = default!;

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    [Required(ErrorMessage = "Section type is required")]
    [StringLength(50, ErrorMessage = "Section type cannot exceed 50 characters")]
    public string SectionType { get; set; } = "General";

    public bool IsVisible { get; set; } = true;
}

public class UpdateLandingPageSectionRequest
{
    [Required(ErrorMessage = "Id is required")]
    public long Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = default!;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = default!;

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    [Required(ErrorMessage = "Section type is required")]
    [StringLength(50, ErrorMessage = "Section type cannot exceed 50 characters")]
    public string SectionType { get; set; } = "General";

    public bool IsVisible { get; set; } = true;
}
