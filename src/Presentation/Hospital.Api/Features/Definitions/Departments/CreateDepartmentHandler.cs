using FluentValidation;
using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.Departments;

public class CreateDepartmentHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<CreateDepartmentRequest> _validator;

    public CreateDepartmentHandler(AppDbContext dbContext, IValidator<CreateDepartmentRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<long>> HandleAsync(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }

        var trimmedName = request.Name.Trim();

        var nameExists = await _dbContext.Departments
            .AnyAsync(d => d.Name.ToLower() == trimmedName.ToLower(), cancellationToken);
        if (nameExists)
        {
            return Result<long>.Failure(Error.Conflict("Department name already exists."));
        }

        var department = new Department
        {
            Name = trimmedName,
            Description = request.Description?.Trim()
        };

        await _dbContext.Departments.AddAsync(department, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(department.Id);
    }
}
