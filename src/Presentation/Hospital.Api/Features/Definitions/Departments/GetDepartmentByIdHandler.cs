using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.Departments;

public class GetDepartmentByIdHandler
{
    private readonly AppDbContext _dbContext;

    public GetDepartmentByIdHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<DepartmentDetailsDto>> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .AsNoTracking()
            .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
            .Include(d => d.DepartmentTerms.Where(t => !t.IsDeleted))
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (department == null)
        {
            return Result<DepartmentDetailsDto>.Failure(Error.NotFound("Department not found."));
        }

        var detailsDto = new DepartmentDetailsDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            Doctors = department.Doctors.Select(doc => new DepartmentDoctorDto
            {
                Id = doc.Id,
                Name = $"{doc.FirstName} {doc.LastName}",
                Specialization = doc.Specialization
            }).ToList(),
            Terms = department.DepartmentTerms.Select(t => new DepartmentTermDto
            {
                Id = t.Id,
                DepartmentId = t.DepartmentId,
                DepartmentName = department.Name,
                Name = t.Name,
                Description = t.Description,
                BaseCost = t.BaseCost
            }).ToList()
        };

        return Result<DepartmentDetailsDto>.Success(detailsDto);
    }
}
