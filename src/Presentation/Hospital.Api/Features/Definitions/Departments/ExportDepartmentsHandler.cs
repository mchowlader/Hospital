using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.Departments;

public class ExportDepartmentsHandler
{
    private readonly AppDbContext _dbContext;

    public ExportDepartmentsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> HandleAsync(
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

        var departments = await query
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                DoctorCount = d.Doctors.Count(doc => !doc.IsDeleted),
                TermCount = d.DepartmentTerms.Count(t => !t.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        var columns = new List<(string Header, Func<DepartmentDto, object?> Selector)>
        {
            ("Department Name", d => d.Name),
            ("Description", d => d.Description),
            ("Doctor Count", d => d.DoctorCount),
            ("Term Count", d => d.TermCount)
        };

        var csvBytes = CsvExporter.ExportToCsv(departments, columns);
        return Result<string>.Success(Convert.ToBase64String(csvBytes));
    }
}
