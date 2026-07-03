using Hospital.Api.Common;
using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Auth;

public class LoginHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public LoginHandler(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<Result<AuthResponse>> HandleAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<AuthResponse>.Failure(Error.Validation("Username/Email and Password are required."));
        }

        // Find user
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(u => u.Username.ToLower() == request.UsernameOrEmail.ToLower() ||
                                       u.Email.ToLower() == request.UsernameOrEmail.ToLower(), cancellationToken);

        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure(Error.Validation("Invalid username or password."));
        }

        if (!user.IsActive)
        {
            return Result<AuthResponse>.Failure(Error.Validation("Your account is deactivated. Please contact the administrator."));
        }

        // Generate JWT Access Token (short-lived: 15 minutes)
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _configuration["Jwt:Key"] ?? "super_secret_key_for_development_hospital_management_123456";
        var key = Encoding.UTF8.GetBytes(jwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.Name)
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = _configuration["Jwt:Issuer"] ?? "Hospital.Api",
            Audience = _configuration["Jwt:Audience"] ?? "Hospital.Client",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Generate cryptographically secure Refresh Token (long-lived: 7 days)
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        var refreshToken = Convert.ToBase64String(randomNumber);

        var userRefreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiryTime = DateTime.UtcNow.AddDays(7),
            IsActive = true,
            IsDeleted = false
        };

        await _dbContext.UserRefreshTokens.AddAsync(userRefreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse
        {
            Token = tokenString,
            RefreshToken = refreshToken,
            Name = user.Name,
            Username = user.Username,
            Email = user.Email,
            Mobile = user.Mobile
        };

        return Result<AuthResponse>.Success(response);
    }
}
