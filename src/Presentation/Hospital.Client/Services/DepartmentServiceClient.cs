using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;

namespace Hospital.Client.Services;

public class DepartmentServiceClient : BaseServiceClient
{
    private const string BaseUrl = "api/departments";

    public DepartmentServiceClient(
        HttpClient httpClient, 
        LocalStorageService localStorage, 
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider)
        : base(httpClient, localStorage, authStateProvider)
    {
    }

    public async Task<Result<PagedResult<DepartmentDto>>> GetDepartmentsAsync(
        int page,
        int size,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}?page={page}&size={size}";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        var response = await HttpClient.GetAsync(url, cancellationToken);
        return await HandleResponseAsync<PagedResult<DepartmentDto>>(response, cancellationToken);
    }

    public async Task<Result<DepartmentDetailsDto>> GetDepartmentByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        return await HandleResponseAsync<DepartmentDetailsDto>(response, cancellationToken);
    }

    public async Task<Result<long>> CreateDepartmentAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        return await HandleResponseAsync<long>(response, cancellationToken);
    }

    public async Task<Result> UpdateDepartmentAsync(long id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    public async Task<Result> DeleteDepartmentAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    public async Task<Result<string>> ExportDepartmentsAsync(string? search, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/export";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"?search={Uri.EscapeDataString(search)}";
        }

        var response = await HttpClient.GetAsync(url, cancellationToken);
        return await HandleResponseAsync<string>(response, cancellationToken);
    }
}
