using Hospital.Api.Common;
using Hospital.Api.Extensions;
using Hospital.Shared.DTOs.LandingPage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.CMS;

public class LandingPageEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/landing-page")
            .WithTags("Landing Page CMS Management");

        group.MapGet("/", GetSectionsAsync)
            .WithName("GetLandingPageSections")
            .AllowAnonymous(); // Anyone can see the landing page!

        group.MapPost("/", CreateSectionAsync)
            .WithName("CreateLandingPageSection")
            .RequirePermission("LandingPageSection.Create");

        group.MapPut("/{id:long}", UpdateSectionAsync)
            .WithName("UpdateLandingPageSection")
            .RequirePermission("LandingPageSection.Edit");

        group.MapDelete("/{id:long}", DeleteSectionAsync)
            .WithName("DeleteLandingPageSection")
            .RequirePermission("LandingPageSection.Delete");
    }

    private static async Task<IResult> GetSectionsAsync(
        bool? includeHidden,
        GetLandingPageSectionsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(includeHidden ?? false, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> CreateSectionAsync(
        CreateLandingPageSectionRequest request,
        CreateLandingPageSectionHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> UpdateSectionAsync(
        long id,
        UpdateLandingPageSectionRequest request,
        UpdateLandingPageSectionHandler handler,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteSectionAsync(
        long id,
        DeleteLandingPageSectionHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }
}
