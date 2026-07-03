using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.DepartmentTerms;

public class GetDepartmentTermsHandler
{
    private readonly AppDbContext _dbContext;

    public GetDepartmentTermsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<DepartmentTermDto>>> HandleAsync(
        int page,
        int size,
        string? search,
        long? departmentId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.DepartmentTerms
            .AsNoTracking()
            .Include(t => t.Department)
            .AsQueryable();

        if (departmentId.HasValue && departmentId.Value > 0)
        {
            query = query.Where(t => t.DepartmentId == departmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(t => t.Name.ToLower().Contains(searchLower) ||
                                     (t.Description != null && t.Description.ToLower().Contains(searchLower)) ||
                                     t.Department.Name.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var terms = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(t => new DepartmentTermDto
            {
                Id = t.Id,
                DepartmentId = t.DepartmentId,
                DepartmentName = t.Department.Name,
                Name = t.Name,
                Description = t.Description,
                BaseCost = t.BaseCost
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<DepartmentTermDto>(terms, totalCount, page, size);
        return Result<PagedResult<DepartmentTermDto>>.Success(pagedResult);
    }
}
