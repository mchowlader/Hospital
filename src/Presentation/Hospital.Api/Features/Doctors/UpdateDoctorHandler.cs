using FluentValidation;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Doctors;

public class UpdateDoctorHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<UpdateDoctorRequest> _validator;

    public UpdateDoctorHandler(AppDbContext dbContext, IValidator<UpdateDoctorRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));

        var doctor = await _dbContext.Doctors
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        if (doctor is null)
            return Result.Failure(Error.NotFound("Doctor not found."));

        // Email conflict check — exclude current doctor
        var emailConflict = await _dbContext.Doctors
            .AnyAsync(d => d.Id != request.Id && d.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (emailConflict)
            return Result.Failure(Error.Conflict("Another doctor with this email already exists."));

        var departmentExists = await _dbContext.Departments
            .AnyAsync(d => d.Id == request.DepartmentId, cancellationToken);
        if (!departmentExists)
            return Result.Failure(Error.NotFound("The selected department does not exist."));

        doctor.FirstName = request.FirstName.Trim();
        doctor.LastName = request.LastName.Trim();
        doctor.Email = request.Email.Trim();
        doctor.PhoneNumber = request.PhoneNumber.Trim();
        doctor.Specialization = request.Specialization.Trim();
        doctor.DepartmentId = request.DepartmentId;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
