using Hospital.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hospital.Api.Common;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IMemoryCache _cache;
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionHandler(IMemoryCache cache, IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return;
        }

        // Check if user is active first (must block deactivated users even in DEBUG mode)
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await dbContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null || !user.IsActive)
            {
                return; // Deny access immediately
            }
        }

#if DEBUG
        // Centralized Bypass: Auto-succeed all permissions in DEBUG mode for smooth testing
        context.Succeed(requirement);
        return;
#endif

#pragma warning disable CS0162 // Unreachable code detected in DEBUG, but active in RELEASE
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
#pragma warning restore CS0162
    }

    private async Task<HashSet<string>> GetUserPermissionsAsync(long userId)
    {
        var cacheKey = $"user-permissions-{userId}";
        if (_cache.TryGetValue(cacheKey, out HashSet<string>? cachedPermissions) && cachedPermissions != null)
        {
            return cachedPermissions;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null || !user.IsActive)
        {
            return new HashSet<string>();
        }

        // 1. Fetch permissions from Roles the user belongs to
        var rolePermissions = await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Key)
            .ToListAsync();

        // 2. Fetch direct User Permission overrides (Allowed)
        var allowedOverrides = await dbContext.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == userId && up.IsAllowed)
            .Select(up => up.Permission.Key)
            .ToListAsync();

        // 3. Fetch direct User Permission overrides (Denied)
        var deniedOverrides = await dbContext.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == userId && !up.IsAllowed)
            .Select(up => up.Permission.Key)
            .ToListAsync();

        // Combine: Roles + Allowed - Denied
        var permissions = rolePermissions
            .Concat(allowedOverrides)
            .Except(deniedOverrides)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Cache for 5 minutes
        _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(5));

        return permissions;
    }
}

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        FallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Dynamically create a policy named after the permission string
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string permission)
    {
        return builder.RequireAuthorization(permission);
    }
}
