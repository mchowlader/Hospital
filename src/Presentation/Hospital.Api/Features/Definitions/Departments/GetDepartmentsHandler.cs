using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.Departments;

public class GetDepartmentsHandler
{
    private readonly AppDbContext _dbContext;

    public GetDepartmentsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<DepartmentDto>>> HandleAsync(
        int page,
        int size,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Departments
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(d => d.Name.ToLower().Contains(searchLower) ||
                                     (d.Description != null && d.Description.ToLower().Contains(searchLower)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var departments = await query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                DoctorCount = d.Doctors.Count(doc => !doc.IsDeleted),
                TermCount = d.DepartmentTerms.Count(t => !t.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<DepartmentDto>(departments, totalCount, page, size);
        return Result<PagedResult<DepartmentDto>>.Success(pagedResult);
    }
}
