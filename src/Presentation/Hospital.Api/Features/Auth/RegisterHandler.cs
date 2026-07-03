using Hospital.Api.Common;
using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Membership;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Auth;

public class RegisterHandler
{
    private readonly AppDbContext _dbContext;

    public RegisterHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<long>> HandleAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Mobile) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<long>.Failure(Error.Validation("All fields are required."));
        }

        // Check Username
        var usernameExists = await _dbContext.Users
            .AnyAsync(u => u.Username.ToLower() == request.Username.ToLower(), cancellationToken);
        if (usernameExists)
        {
            return Result<long>.Failure(Error.Conflict("Username is already taken."));
        }

        // Check Email
        var emailExists = await _dbContext.Users
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);
        if (emailExists)
        {
            return Result<long>.Failure(Error.Conflict("Email is already registered."));
        }

        var user = new User
        {
            Name = request.Name,
            Username = request.Username,
            Email = request.Email,
            Mobile = request.Mobile,
            PasswordHash = PasswordHasher.Hash(request.Password)
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Assign default Staff role
        var staffRole = await _dbContext.Roles
            .SingleOrDefaultAsync(r => r.Name == "Staff", cancellationToken);
        if (staffRole != null)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = staffRole.Id
            };
            await _dbContext.UserRoles.AddAsync(userRole, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result<long>.Success(user.Id);
    }
}
