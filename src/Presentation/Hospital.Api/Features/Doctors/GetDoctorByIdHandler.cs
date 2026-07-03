using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Doctors;

public class GetDoctorByIdHandler
{
    private readonly AppDbContext _dbContext;

    public GetDoctorByIdHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<DoctorDetailsDto>> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var doctor = await _dbContext.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .Where(d => d.Id == id)
            .Select(d => new DoctorDetailsDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                Specialization = d.Specialization,
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Department.Name,
                UserId = d.UserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return Result<DoctorDetailsDto>.Failure(Error.NotFound("Doctor not found."));

        return Result<DoctorDetailsDto>.Success(doctor);
    }
}
