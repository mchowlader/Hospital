using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Patients;

namespace Hospital.Client.Services;

public class PatientServiceClient : BaseServiceClient
{
    private const string BaseUrl = "api/patients";

    public PatientServiceClient(
        HttpClient httpClient,
        LocalStorageService localStorage,
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider)
        : base(httpClient, localStorage, authStateProvider)
    {
    }

    public async Task<Result<PagedResult<PatientDto>>> GetPatientsAsync(
        int page,
        int size,
        string? search,
        string? gender = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}?page={page}&size={size}";
        if (!string.IsNullOrWhiteSpace(gender))
            url += $"&gender={Uri.EscapeDataString(gender)}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        var response = await HttpClient.GetAsync(url, cancellationToken);
        return await HandleResponseAsync<PagedResult<PatientDto>>(response, cancellationToken);
    }

    public async Task<Result<PatientDetailsDto>> GetPatientByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        return await HandleResponseAsync<PatientDetailsDto>(response, cancellationToken);
    }

    public async Task<Result<long>> CreatePatientAsync(CreatePatientRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        return await HandleResponseAsync<long>(response, cancellationToken);
    }

    public async Task<Result> UpdatePatientAsync(long id, UpdatePatientRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    public async Task<Result> DeletePatientAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }
}
