using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Membership;
using Microsoft.AspNetCore.Http;
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

public class RefreshHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshHandler(
        AppDbContext dbContext,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<AuthResponse>> HandleAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result<AuthResponse>.Failure(Error.Validation("Access token and Refresh token are required."));
        }

        ClaimsPrincipal principal;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? "super_secret_key_for_development_hospital_management_123456";
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Decode even if expired
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"] ?? "Hospital.Api",
                ValidAudience = _configuration["Jwt:Audience"] ?? "Hospital.Client",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var claimsPrincipal = tokenHandler.ValidateToken(request.AccessToken, validationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Result<AuthResponse>.Failure(Error.Validation("Invalid token algorithms."));
            }

            principal = claimsPrincipal;
        }
        catch (Exception)
        {
            return Result<AuthResponse>.Failure(Error.Validation("Invalid access token format."));
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return Result<AuthResponse>.Failure(Error.Validation("Invalid token claims."));
        }

        var user = await _dbContext.Users
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return Result<AuthResponse>.Failure(Error.NotFound("User not found."));
        }

        if (!user.IsActive)
        {
            return Result<AuthResponse>.Failure(Error.Validation("User account is deactivated."));
        }

        // Validate the refresh token from database
        var storedRefreshToken = await _dbContext.UserRefreshTokens
            .SingleOrDefaultAsync(urt => urt.UserId == userId &&
                                         urt.Token == request.RefreshToken, cancellationToken);

        if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpiryTime <= DateTime.UtcNow)
        {
            return Result<AuthResponse>.Failure(Error.Validation("Invalid or expired refresh token."));
        }

        // Revoke the old refresh token (Refresh Token Rotation)
        storedRefreshToken.IsRevoked = true;

        // Generate new Access Token
        var newTokenHandler = new JwtSecurityTokenHandler();
        var newJwtKey = _configuration["Jwt:Key"] ?? "super_secret_key_for_development_hospital_management_123456";
        var newKey = Encoding.UTF8.GetBytes(newJwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(principal.Claims),
            Expires = DateTime.UtcNow.AddMinutes(15), // New access token expires in 15 minutes
            Issuer = _configuration["Jwt:Issuer"] ?? "Hospital.Api",
            Audience = _configuration["Jwt:Audience"] ?? "Hospital.Client",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(newKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var newAccessToken = newTokenHandler.CreateToken(tokenDescriptor);
        var newAccessTokenString = newTokenHandler.WriteToken(newAccessToken);

        // Generate new Refresh Token
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        var newRefreshToken = Convert.ToBase64String(randomNumber);

        var clientIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        var newUserRefreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiryTime = DateTime.UtcNow.AddDays(7),
            CreatedByIp = clientIp,
            IsActive = true,
            IsDeleted = false
        };

        await _dbContext.UserRefreshTokens.AddAsync(newUserRefreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse
        {
            Token = newAccessTokenString,
            RefreshToken = newRefreshToken,
            Name = user.Name,
            Username = user.Username,
            Email = user.Email,
            Mobile = user.Mobile
        };

        return Result<AuthResponse>.Success(response);
    }
}
