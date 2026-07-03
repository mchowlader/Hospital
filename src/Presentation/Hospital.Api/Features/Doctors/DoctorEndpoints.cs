using Hospital.Api.Common;
using Hospital.Api.Extensions;
using Hospital.Shared.DTOs.Doctors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Doctors;

public class DoctorEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/doctors")
            .WithTags("Doctors Management");

        group.MapGet("/", GetDoctorsAsync)
            .WithName("GetDoctors")
            .RequirePermission("Doctor.View");

        group.MapGet("/{id:long}", GetDoctorByIdAsync)
            .WithName("GetDoctorById")
            .RequirePermission("Doctor.View");

        group.MapPost("/", CreateDoctorAsync)
            .WithName("CreateDoctor")
            .RequirePermission("Doctor.Create");

        group.MapPut("/{id:long}", UpdateDoctorAsync)
            .WithName("UpdateDoctor")
            .RequirePermission("Doctor.Edit");

        group.MapDelete("/{id:long}", DeleteDoctorAsync)
            .WithName("DeleteDoctor")
            .RequirePermission("Doctor.Delete");
    }

    private static async Task<IResult> GetDoctorsAsync(
        int? page,
        int? size,
        string? search,
        long? departmentId,
        GetDoctorsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(page ?? 1, size ?? 10, search, departmentId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }

    private static async Task<IResult> GetDoctorByIdAsync(
        long id,
        GetDoctorByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }

    private static async Task<IResult> CreateDoctorAsync(
        CreateDoctorRequest request,
        CreateDoctorHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }

    private static async Task<IResult> UpdateDoctorAsync(
        long id,
        UpdateDoctorRequest request,
        UpdateDoctorHandler handler,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteDoctorAsync(
        long id,
        DeleteDoctorHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }
}
