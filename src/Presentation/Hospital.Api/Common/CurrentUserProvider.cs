using Hospital.Domain.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Hospital.Api.Common;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUsername()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            return "system";
        }

        var username = context.User.FindFirstValue(ClaimTypes.Name)
                       ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return username ?? "system";
    }

    public long? GetCurrentUserId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            return null;
        }

        var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (long.TryParse(userIdString, out var userId))
        {
            return userId;
        }

        return null;
    }
}
