using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Shared.DTOs.Departments;

public class DepartmentDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int DoctorCount { get; set; }
    public int TermCount { get; set; }
}

public class CreateDepartmentRequest
{
    [Required(ErrorMessage = "Department name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 100 characters")]
    public string Name { get; set; } = default!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}

public class UpdateDepartmentRequest
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Department name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 100 characters")]
    public string Name { get; set; } = default!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}

public class DepartmentDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<DepartmentDoctorDto> Doctors { get; set; } = new();
    public List<DepartmentTermDto> Terms { get; set; } = new();
}

public class DepartmentDoctorDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string Specialization { get; set; } = default!;
}
