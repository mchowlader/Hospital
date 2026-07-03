using FluentValidation;

namespace Hospital.Shared.DTOs.Departments;

public class CreateDepartmentTermValidator : AbstractValidator<CreateDepartmentTermRequest>
{
    public CreateDepartmentTermValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Term name is required")
            .Length(2, 100).WithMessage("Term name must be between 2 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.BaseCost)
            .GreaterThanOrEqualTo(0).WithMessage("Base cost must be a non-negative value");
    }
}

public class UpdateDepartmentTermValidator : AbstractValidator<UpdateDepartmentTermRequest>
{
    public UpdateDepartmentTermValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Term name is required")
            .Length(2, 100).WithMessage("Term name must be between 2 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.BaseCost)
            .GreaterThanOrEqualTo(0).WithMessage("Base cost must be a non-negative value");
    }
}
