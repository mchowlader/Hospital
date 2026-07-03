using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.CMS;

public class DeleteLandingPageSectionHandler
{
    private readonly AppDbContext _dbContext;

    public DeleteLandingPageSectionHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var section = await _dbContext.LandingPageSections
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (section == null)
        {
            return Result.Failure(Error.NotFound("Landing page section not found."));
        }

        // Soft delete the section (handled automatically by AppDbContext interceptor on Remove)
        _dbContext.LandingPageSections.Remove(section);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
