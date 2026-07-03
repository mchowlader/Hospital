using FluentValidation;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Patients;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Patients;

public class UpdatePatientHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<UpdatePatientRequest> _validator;

    public UpdatePatientHandler(AppDbContext dbContext, IValidator<UpdatePatientRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));

        var patient = await _dbContext.Patients
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (patient is null)
            return Result.Failure(Error.NotFound("Patient not found."));

        var emailConflict = await _dbContext.Patients
            .AnyAsync(p => p.Id != request.Id && p.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (emailConflict)
            return Result.Failure(Error.Conflict("Another patient with this email already exists."));

        var phoneConflict = await _dbContext.Patients
            .AnyAsync(p => p.Id != request.Id && p.PhoneNumber == request.PhoneNumber.Trim(), cancellationToken);
        if (phoneConflict)
            return Result.Failure(Error.Conflict("Another patient with this phone number already exists."));

        patient.FirstName = request.FirstName.Trim();
        patient.LastName = request.LastName.Trim();
        patient.Email = request.Email.Trim();
        patient.PhoneNumber = request.PhoneNumber.Trim();
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender;
        patient.MedicalHistorySummary = request.MedicalHistorySummary?.Trim();

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
