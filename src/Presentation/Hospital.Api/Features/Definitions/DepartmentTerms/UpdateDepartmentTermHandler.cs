using FluentValidation;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.DepartmentTerms;

public class UpdateDepartmentTermHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<UpdateDepartmentTermRequest> _validator;

    public UpdateDepartmentTermHandler(AppDbContext dbContext, IValidator<UpdateDepartmentTermRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateDepartmentTermRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }

        var term = await _dbContext.DepartmentTerms
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (term == null)
        {
            return Result.Failure(Error.NotFound("Department Term not found."));
        }

        // Validate department exists
        var departmentExists = await _dbContext.Departments
            .AnyAsync(d => d.Id == request.DepartmentId, cancellationToken);
        if (!departmentExists)
        {
            return Result.Failure(Error.NotFound("Department not found."));
        }

        var trimmedName = request.Name.Trim();

        var nameExists = await _dbContext.DepartmentTerms
            .AnyAsync(t => t.Id != request.Id && t.DepartmentId == request.DepartmentId && t.Name.ToLower() == trimmedName.ToLower(), cancellationToken);
        if (nameExists)
        {
            return Result.Failure(Error.Conflict("Another Term with the same name already exists in this department."));
        }

        term.DepartmentId = request.DepartmentId;
        term.Name = trimmedName;
        term.Description = request.Description?.Trim();
        term.BaseCost = request.BaseCost;

        _dbContext.DepartmentTerms.Update(term);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
