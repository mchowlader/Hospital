using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Patients;

public class DeletePatientHandler
{
    private readonly AppDbContext _dbContext;

    public DeletePatientHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.Patients
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (patient is null)
            return Result.Failure(Error.NotFound("Patient not found."));

        patient.IsDeleted = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
