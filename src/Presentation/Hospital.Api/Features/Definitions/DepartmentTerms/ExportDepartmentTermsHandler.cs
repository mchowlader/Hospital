using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.DepartmentTerms;

public class ExportDepartmentTermsHandler
{
    private readonly AppDbContext _dbContext;

    public ExportDepartmentTermsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> HandleAsync(
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

        var terms = await query
            .OrderBy(t => t.Name)
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

        var columns = new List<(string Header, Func<DepartmentTermDto, object?> Selector)>
        {
            ("Department", t => t.DepartmentName),
            ("Term Name", t => t.Name),
            ("Description", t => t.Description),
            ("Base Cost", t => t.BaseCost)
        };

        var csvBytes = CsvExporter.ExportToCsv(terms, columns);
        return Result<string>.Success(Convert.ToBase64String(csvBytes));
    }
}
