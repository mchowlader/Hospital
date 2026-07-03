using Hospital.Api.Common;
using Hospital.Api.Extensions;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.DepartmentTerms;

public class DepartmentTermEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/department-terms")
            .WithTags("Department Terms Management");

        group.MapGet("/", GetDepartmentTermsAsync)
            .WithName("GetDepartmentTerms")
            .RequirePermission("DepartmentTerm.View");

        group.MapGet("/{id:long}", GetDepartmentTermByIdAsync)
            .WithName("GetDepartmentTermById")
            .RequirePermission("DepartmentTerm.View");

        group.MapGet("/export", ExportDepartmentTermsAsync)
            .WithName("ExportDepartmentTerms")
            .RequirePermission("DepartmentTerm.View");

        group.MapPost("/", CreateDepartmentTermAsync)
            .WithName("CreateDepartmentTerm")
            .RequirePermission("DepartmentTerm.Create");

        group.MapPut("/{id:long}", UpdateDepartmentTermAsync)
            .WithName("UpdateDepartmentTerm")
            .RequirePermission("DepartmentTerm.Edit");

        group.MapDelete("/{id:long}", DeleteDepartmentTermAsync)
            .WithName("DeleteDepartmentTerm")
            .RequirePermission("DepartmentTerm.Delete");
    }

    private static async Task<IResult> GetDepartmentTermsAsync(
        int? page,
        int? size,
        string? search,
        long? departmentId,
        GetDepartmentTermsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(page ?? 1, size ?? 10, search, departmentId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetDepartmentTermByIdAsync(
        long id,
        GetDepartmentTermByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> ExportDepartmentTermsAsync(
        string? search,
        long? departmentId,
        ExportDepartmentTermsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(search, departmentId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> CreateDepartmentTermAsync(
        CreateDepartmentTermRequest request,
        CreateDepartmentTermHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> UpdateDepartmentTermAsync(
        long id,
        UpdateDepartmentTermRequest request,
        UpdateDepartmentTermHandler handler,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteDepartmentTermAsync(
        long id,
        DeleteDepartmentTermHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }
}
