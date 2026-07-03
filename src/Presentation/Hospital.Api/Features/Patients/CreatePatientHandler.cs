using FluentValidation;
using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Patients;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Patients;

public class CreatePatientHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<CreatePatientRequest> _validator;

    public CreatePatientHandler(AppDbContext dbContext, IValidator<CreatePatientRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<long>> HandleAsync(CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<long>.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));

        var emailExists = await _dbContext.Patients
            .AnyAsync(p => p.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (emailExists)
            return Result<long>.Failure(Error.Conflict("A patient with this email already exists."));

        var phoneExists = await _dbContext.Patients
            .AnyAsync(p => p.PhoneNumber == request.PhoneNumber.Trim(), cancellationToken);
        if (phoneExists)
            return Result<long>.Failure(Error.Conflict("A patient with this phone number already exists."));

        var patient = new Patient
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            MedicalHistorySummary = request.MedicalHistorySummary?.Trim()
        };

        await _dbContext.Patients.AddAsync(patient, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(patient.Id);
    }
}
