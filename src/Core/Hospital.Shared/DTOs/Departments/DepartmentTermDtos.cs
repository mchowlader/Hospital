using System.ComponentModel.DataAnnotations;

namespace Hospital.Shared.DTOs.Departments;

public class DepartmentTermDto
{
    public long Id { get; set; }
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal BaseCost { get; set; }
}

public class CreateDepartmentTermRequest
{
    [Required(ErrorMessage = "Department is required")]
    public long DepartmentId { get; set; }

    [Required(ErrorMessage = "Term name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Term name must be between 2 and 100 characters")]
    public string Name { get; set; } = default!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Range(0, 1000000, ErrorMessage = "Base cost must be a non-negative value")]
    public decimal BaseCost { get; set; }
}

public class UpdateDepartmentTermRequest
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Department is required")]
    public long DepartmentId { get; set; }

    [Required(ErrorMessage = "Term name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Term name must be between 2 and 100 characters")]
    public string Name { get; set; } = default!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Range(0, 1000000, ErrorMessage = "Base cost must be a non-negative value")]
    public decimal BaseCost { get; set; }
}
