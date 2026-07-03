using Hospital.Api.Common;
using Hospital.Api.Extensions;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Definitions.Departments;

public class DepartmentEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/departments")
            .WithTags("Departments Management");

        group.MapGet("/", GetDepartmentsAsync)
            .WithName("GetDepartments")
            .RequirePermission("Department.View");

        group.MapGet("/{id:long}", GetDepartmentByIdAsync)
            .WithName("GetDepartmentById")
            .RequirePermission("Department.View");

        group.MapGet("/export", ExportDepartmentsAsync)
            .WithName("ExportDepartments")
            .RequirePermission("Department.View");

        group.MapPost("/", CreateDepartmentAsync)
            .WithName("CreateDepartment")
            .RequirePermission("Department.Create");

        group.MapPut("/{id:long}", UpdateDepartmentAsync)
            .WithName("UpdateDepartment")
            .RequirePermission("Department.Edit");

        group.MapDelete("/{id:long}", DeleteDepartmentAsync)
            .WithName("DeleteDepartment")
            .RequirePermission("Department.Delete");
    }

    private static async Task<IResult> GetDepartmentsAsync(
        int? page,
        int? size,
        string? search,
        GetDepartmentsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(page ?? 1, size ?? 10, search, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetDepartmentByIdAsync(
        long id,
        GetDepartmentByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> ExportDepartmentsAsync(
        string? search,
        ExportDepartmentsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(search, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> CreateDepartmentAsync(
        CreateDepartmentRequest request,
        CreateDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> UpdateDepartmentAsync(
        long id,
        UpdateDepartmentRequest request,
        UpdateDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteDepartmentAsync(
        long id,
        DeleteDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }
}
