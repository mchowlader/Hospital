using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Patients;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Patients;

public class GetPatientByIdHandler
{
    private readonly AppDbContext _dbContext;

    public GetPatientByIdHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PatientDetailsDto>> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.Patients
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PatientDetailsDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                MedicalHistorySummary = p.MedicalHistorySummary,
                UserId = p.UserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result<PatientDetailsDto>.Failure(Error.NotFound("Patient not found."));

        return Result<PatientDetailsDto>.Success(patient);
    }
}
