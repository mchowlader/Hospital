using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.Departments;

public class DeleteDepartmentHandler
{
    private readonly AppDbContext _dbContext;

    public DeleteDepartmentHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (department == null)
        {
            return Result.Failure(Error.NotFound("Department not found."));
        }

        var hasActiveDoctors = await _dbContext.Doctors
            .AnyAsync(doc => doc.DepartmentId == id && !doc.IsDeleted, cancellationToken);

        if (hasActiveDoctors)
        {
            return Result.Failure(Error.Conflict("Cannot delete department because it contains active doctors."));
        }

        // Soft delete the department (handled automatically by AppDbContext interceptor on Remove)
        _dbContext.Departments.Remove(department);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
