using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Patients;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Patients;

public class GetPatientsHandler
{
    private readonly AppDbContext _dbContext;

    public GetPatientsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<PatientDto>>> HandleAsync(
        int page,
        int size,
        string? search,
        string? gender,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Patients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(gender))
            query = query.Where(p => p.Gender.ToLower() == gender.ToLower());

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(s) ||
                p.LastName.ToLower().Contains(s) ||
                p.Email.ToLower().Contains(s) ||
                p.PhoneNumber.Contains(s));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var patients = await query
            .OrderBy(p => p.FirstName).ThenBy(p => p.LastName)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<PatientDto>>.Success(
            new PagedResult<PatientDto>(patients, totalCount, page, size));
    }
}
