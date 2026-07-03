using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Shared.DTOs.Patients;

public class PatientDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public int Age => (int)((DateTime.UtcNow - DateOfBirth).TotalDays / 365.25);
    public string Gender { get; set; } = default!;
}

public class PatientDetailsDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public int Age => (int)((DateTime.UtcNow - DateOfBirth).TotalDays / 365.25);
    public string Gender { get; set; } = default!;
    public string? MedicalHistorySummary { get; set; }
    public long? UserId { get; set; }
}

public class CreatePatientRequest
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

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-25);

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = "Male";

    [StringLength(1000, ErrorMessage = "Medical history cannot exceed 1000 characters")]
    public string? MedicalHistorySummary { get; set; }
}

public class UpdatePatientRequest
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

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = default!;

    [StringLength(1000, ErrorMessage = "Medical history cannot exceed 1000 characters")]
    public string? MedicalHistorySummary { get; set; }
}
