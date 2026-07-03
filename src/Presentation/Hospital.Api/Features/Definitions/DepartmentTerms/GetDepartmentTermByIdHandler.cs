using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.DepartmentTerms;

public class GetDepartmentTermByIdHandler
{
    private readonly AppDbContext _dbContext;

    public GetDepartmentTermByIdHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<DepartmentTermDto>> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var term = await _dbContext.DepartmentTerms
            .AsNoTracking()
            .Include(t => t.Department)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (term == null)
        {
            return Result<DepartmentTermDto>.Failure(Error.NotFound("Department Term not found."));
        }

        var dto = new DepartmentTermDto
        {
            Id = term.Id,
            DepartmentId = term.DepartmentId,
            DepartmentName = term.Department.Name,
            Name = term.Name,
            Description = term.Description,
            BaseCost = term.BaseCost
        };

        return Result<DepartmentTermDto>.Success(dto);
    }
}
