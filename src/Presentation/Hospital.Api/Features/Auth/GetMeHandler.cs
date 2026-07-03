using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Membership;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Auth;

public class GetMeHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMeHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<UserMeResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var userPrincipal = _httpContextAccessor.HttpContext?.User;
        if (userPrincipal == null)
        {
            return Result<UserMeResponse>.Failure(Error.Unauthorized("User is not authenticated."));
        }

        var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return Result<UserMeResponse>.Failure(Error.Unauthorized("User identity claim not found."));
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return Result<UserMeResponse>.Failure(Error.NotFound("User not found."));
        }

        if (!user.IsActive)
        {
            return Result<UserMeResponse>.Failure(Error.Unauthorized("Your account has been deactivated."));
        }

        // Fetch user roles
        var roles = await _dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        // Calculate permissions (Roles + Allowed - Denied)
        var rolePermissions = await _dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Key)
            .ToListAsync(cancellationToken);

        var allowedOverrides = await _dbContext.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == userId && up.IsAllowed)
            .Select(up => up.Permission.Key)
            .ToListAsync(cancellationToken);

        var deniedOverrides = await _dbContext.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == userId && !up.IsAllowed)
            .Select(up => up.Permission.Key)
            .ToListAsync(cancellationToken);

        var permissions = rolePermissions
            .Concat(allowedOverrides)
            .Except(deniedOverrides)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Check if the user is a doctor or a patient
        var doctorId = await _dbContext.Doctors
            .AsNoTracking()
            .Where(d => d.UserId == userId)
            .Select(d => (long?)d.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var patientId = await _dbContext.Patients
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => (long?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var response = new UserMeResponse
        {
            Id = user.Id,
            Name = user.Name,
            Username = user.Username,
            Email = user.Email,
            Mobile = user.Mobile,
            DoctorId = doctorId,
            PatientId = patientId,
            Roles = roles,
            Permissions = permissions
        };

        return Result<UserMeResponse>.Success(response);
    }
}
