using Hospital.Api.Common;
using Hospital.Api.Extensions;
using Hospital.Shared.DTOs.Patients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.Patients;

public class PatientEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patients")
            .WithTags("Patients Management");

        group.MapGet("/", GetPatientsAsync)
            .WithName("GetPatients")
            .RequirePermission("Patient.View");

        group.MapGet("/{id:long}", GetPatientByIdAsync)
            .WithName("GetPatientById")
            .RequirePermission("Patient.View");

        group.MapPost("/", CreatePatientAsync)
            .WithName("CreatePatient")
            .RequirePermission("Patient.Create");

        group.MapPut("/{id:long}", UpdatePatientAsync)
            .WithName("UpdatePatient")
            .RequirePermission("Patient.Edit");

        group.MapDelete("/{id:long}", DeletePatientAsync)
            .WithName("DeletePatient")
            .RequirePermission("Patient.Delete");
    }

    private static async Task<IResult> GetPatientsAsync(
        int? page,
        int? size,
        string? search,
        string? gender,
        GetPatientsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(page ?? 1, size ?? 10, search, gender, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }

    private static async Task<IResult> GetPatientByIdAsync(
        long id,
        GetPatientByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }

    private static async Task<IResult> CreatePatientAsync(
        CreatePatientRequest request,
        CreatePatientHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }

    private static async Task<IResult> UpdatePatientAsync(
        long id,
        UpdatePatientRequest request,
        UpdatePatientHandler handler,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }

    private static async Task<IResult> DeletePatientAsync(
        long id,
        DeletePatientHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : result.ToProblemDetails();
    }
}
