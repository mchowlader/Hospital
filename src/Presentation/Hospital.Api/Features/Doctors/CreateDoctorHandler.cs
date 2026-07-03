using FluentValidation;
using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Doctors;

public class CreateDoctorHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<CreateDoctorRequest> _validator;

    public CreateDoctorHandler(AppDbContext dbContext, IValidator<CreateDoctorRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<long>> HandleAsync(CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<long>.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));

        var emailExists = await _dbContext.Doctors
            .AnyAsync(d => d.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (emailExists)
            return Result<long>.Failure(Error.Conflict("A doctor with this email already exists."));

        var departmentExists = await _dbContext.Departments
            .AnyAsync(d => d.Id == request.DepartmentId, cancellationToken);
        if (!departmentExists)
            return Result<long>.Failure(Error.NotFound("The selected department does not exist."));

        var doctor = new Doctor
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Specialization = request.Specialization.Trim(),
            DepartmentId = request.DepartmentId
        };

        await _dbContext.Doctors.AddAsync(doctor, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(doctor.Id);
    }
}
