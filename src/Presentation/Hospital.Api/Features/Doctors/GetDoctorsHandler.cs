using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Doctors;

public class GetDoctorsHandler
{
    private readonly AppDbContext _dbContext;

    public GetDoctorsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<DoctorDto>>> HandleAsync(
        int page,
        int size,
        string? search,
        long? departmentId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .AsQueryable();

        if (departmentId.HasValue && departmentId.Value > 0)
            query = query.Where(d => d.DepartmentId == departmentId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(d =>
                d.FirstName.ToLower().Contains(s) ||
                d.LastName.ToLower().Contains(s) ||
                d.Email.ToLower().Contains(s) ||
                d.Specialization.ToLower().Contains(s));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var doctors = await query
            .OrderBy(d => d.FirstName).ThenBy(d => d.LastName)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(d => new DoctorDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                Specialization = d.Specialization,
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Department.Name
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<DoctorDto>>.Success(
            new PagedResult<DoctorDto>(doctors, totalCount, page, size));
    }
}
