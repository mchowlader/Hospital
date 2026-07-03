using Hospital.Api.Common;
using Hospital.Api.Extensions;
using Hospital.Shared.DTOs.Membership;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Auth;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication & Membership");

        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .AllowAnonymous();

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .AllowAnonymous();

        group.MapPost("/refresh", RefreshAsync)
            .WithName("Refresh")
            .AllowAnonymous();

        group.MapGet("/me", GetMeAsync)
            .WithName("GetMe");
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        RegisterHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        LoginHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> RefreshAsync(
        RefreshTokenRequest request,
        RefreshHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetMeAsync(
        GetMeHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }
}
