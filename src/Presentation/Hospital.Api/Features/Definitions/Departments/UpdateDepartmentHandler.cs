using FluentValidation;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.Departments;

public class UpdateDepartmentHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<UpdateDepartmentRequest> _validator;

    public UpdateDepartmentHandler(AppDbContext dbContext, IValidator<UpdateDepartmentRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }

        var department = await _dbContext.Departments
            .SingleOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
        {
            return Result.Failure(Error.NotFound("Department not found."));
        }

        var trimmedName = request.Name.Trim();

        var nameExists = await _dbContext.Departments
            .AnyAsync(d => d.Id != request.Id && d.Name.ToLower() == trimmedName.ToLower(), cancellationToken);
        if (nameExists)
        {
            return Result.Failure(Error.Conflict("Another department with the same name already exists."));
        }

        department.Name = trimmedName;
        department.Description = request.Description?.Trim();

        _dbContext.Departments.Update(department);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
