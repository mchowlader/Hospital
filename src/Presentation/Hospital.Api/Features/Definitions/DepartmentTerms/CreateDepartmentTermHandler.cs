using FluentValidation;
using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.DepartmentTerms;

public class CreateDepartmentTermHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<CreateDepartmentTermRequest> _validator;

    public CreateDepartmentTermHandler(AppDbContext dbContext, IValidator<CreateDepartmentTermRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<long>> HandleAsync(CreateDepartmentTermRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }

        // Validate department exists
        var departmentExists = await _dbContext.Departments
            .AnyAsync(d => d.Id == request.DepartmentId, cancellationToken);
        if (!departmentExists)
        {
            return Result<long>.Failure(Error.NotFound("Department not found."));
        }

        var trimmedName = request.Name.Trim();

        var nameExists = await _dbContext.DepartmentTerms
            .AnyAsync(t => t.DepartmentId == request.DepartmentId && t.Name.ToLower() == trimmedName.ToLower(), cancellationToken);
        if (nameExists)
        {
            return Result<long>.Failure(Error.Conflict("Department Term name already exists within this department."));
        }

        var term = new DepartmentTerm
        {
            DepartmentId = request.DepartmentId,
            Name = trimmedName,
            Description = request.Description?.Trim(),
            BaseCost = request.BaseCost
        };

        await _dbContext.DepartmentTerms.AddAsync(term, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(term.Id);
    }
}
