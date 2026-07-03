using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Shared.DTOs.Doctors;

public class DoctorDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Specialization { get; set; } = default!;
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = default!;
}

public class DoctorDetailsDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Specialization { get; set; } = default!;
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = default!;
    public long? UserId { get; set; }
}

public class CreateDoctorRequest
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be 2–50 characters")]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be 2–50 characters")]
    public string LastName { get; set; } = default!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string PhoneNumber { get; set; } = default!;

    [Required(ErrorMessage = "Specialization is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Specialization must be 2–100 characters")]
    public string Specialization { get; set; } = default!;

    [Required(ErrorMessage = "Department is required")]
    [Range(1, long.MaxValue, ErrorMessage = "Please select a valid department")]
    public long DepartmentId { get; set; }
}

public class UpdateDoctorRequest
{
    public long Id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be 2–50 characters")]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be 2–50 characters")]
    public string LastName { get; set; } = default!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string PhoneNumber { get; set; } = default!;

    [Required(ErrorMessage = "Specialization is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Specialization must be 2–100 characters")]
    public string Specialization { get; set; } = default!;

    [Required(ErrorMessage = "Department is required")]
    [Range(1, long.MaxValue, ErrorMessage = "Please select a valid department")]
    public long DepartmentId { get; set; }
}
