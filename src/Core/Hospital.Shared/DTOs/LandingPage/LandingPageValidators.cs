using FluentValidation;

namespace Hospital.Shared.DTOs.LandingPage;

public class CreateLandingPageSectionValidator : AbstractValidator<CreateLandingPageSectionRequest>
{
    public CreateLandingPageSectionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.SectionType)
            .NotEmpty().WithMessage("Section type is required")
            .MaximumLength(50).WithMessage("Section type cannot exceed 50 characters");
    }
}

public class UpdateLandingPageSectionValidator : AbstractValidator<UpdateLandingPageSectionRequest>
{
    public UpdateLandingPageSectionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.SectionType)
            .NotEmpty().WithMessage("Section type is required")
            .MaximumLength(50).WithMessage("Section type cannot exceed 50 characters");
    }
}
