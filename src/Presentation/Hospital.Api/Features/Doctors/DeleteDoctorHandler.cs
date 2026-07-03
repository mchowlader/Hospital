using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Doctors;

public class DeleteDoctorHandler
{
    private readonly AppDbContext _dbContext;

    public DeleteDoctorHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var doctor = await _dbContext.Doctors
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doctor is null)
            return Result.Failure(Error.NotFound("Doctor not found."));

        doctor.IsDeleted = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
