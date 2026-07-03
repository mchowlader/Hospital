using FluentValidation;
using System;

namespace Hospital.Shared.DTOs.Patients;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    private static readonly string[] ValidGenders = ["Male", "Female", "Other"];

    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(150).WithMessage("Email cannot exceed 150 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.UtcNow.AddYears(-150)).WithMessage("Date of birth seems too old");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(g => Array.Exists(ValidGenders, v => v.Equals(g, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Gender must be Male, Female, or Other");

        RuleFor(x => x.MedicalHistorySummary)
            .MaximumLength(1000).WithMessage("Medical history cannot exceed 1000 characters")
            .When(x => x.MedicalHistorySummary != null);
    }
}

public class UpdatePatientRequestValidator : AbstractValidator<UpdatePatientRequest>
{
    private static readonly string[] ValidGenders = ["Male", "Female", "Other"];

    public UpdatePatientRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid patient ID");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(150).WithMessage("Email cannot exceed 150 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.UtcNow.AddYears(-150)).WithMessage("Date of birth seems too old");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(g => Array.Exists(ValidGenders, v => v.Equals(g, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Gender must be Male, Female, or Other");

        RuleFor(x => x.MedicalHistorySummary)
            .MaximumLength(1000).WithMessage("Medical history cannot exceed 1000 characters")
            .When(x => x.MedicalHistorySummary != null);
    }
}
