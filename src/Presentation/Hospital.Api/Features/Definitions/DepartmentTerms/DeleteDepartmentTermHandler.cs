using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.DepartmentTerms;

public class DeleteDepartmentTermHandler
{
    private readonly AppDbContext _dbContext;

    public DeleteDepartmentTermHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var term = await _dbContext.DepartmentTerms
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (term == null)
        {
            return Result.Failure(Error.NotFound("Department Term not found."));
        }

        var hasActiveTreatments = await _dbContext.TreatmentDetails
            .AnyAsync(td => td.DepartmentTermId == id && !td.IsDeleted, cancellationToken);

        if (hasActiveTreatments)
        {
            return Result.Failure(Error.Conflict("Cannot delete this treatment term because it is linked to existing treatment records."));
        }

        // Soft delete the term (handled automatically by AppDbContext interceptor on Remove)
        _dbContext.DepartmentTerms.Remove(term);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
