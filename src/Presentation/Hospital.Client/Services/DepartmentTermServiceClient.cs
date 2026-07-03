using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Departments;

namespace Hospital.Client.Services;

public class DepartmentTermServiceClient : BaseServiceClient
{
    private const string BaseUrl = "api/department-terms";

    public DepartmentTermServiceClient(
        HttpClient httpClient, 
        LocalStorageService localStorage, 
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider)
        : base(httpClient, localStorage, authStateProvider)
    {
    }

    public async Task<Result<PagedResult<DepartmentTermDto>>> GetDepartmentTermsAsync(
        int page,
        int size,
        string? search,
        long? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}?page={page}&size={size}";
        if (departmentId.HasValue && departmentId.Value > 0)
        {
            url += $"&departmentId={departmentId.Value}";
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        var response = await HttpClient.GetAsync(url, cancellationToken);
        return await HandleResponseAsync<PagedResult<DepartmentTermDto>>(response, cancellationToken);
    }

    public async Task<Result<DepartmentTermDto>> GetDepartmentTermByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        return await HandleResponseAsync<DepartmentTermDto>(response, cancellationToken);
    }

    public async Task<Result<long>> CreateDepartmentTermAsync(CreateDepartmentTermRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        return await HandleResponseAsync<long>(response, cancellationToken);
    }

    public async Task<Result> UpdateDepartmentTermAsync(long id, UpdateDepartmentTermRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    public async Task<Result> DeleteDepartmentTermAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    public async Task<Result<string>> ExportDepartmentTermsAsync(
        string? search, 
        long? departmentId = null, 
        CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/export?";
        if (departmentId.HasValue && departmentId.Value > 0)
        {
            url += $"&departmentId={departmentId.Value}";
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        var response = await HttpClient.GetAsync(url, cancellationToken);
        return await HandleResponseAsync<string>(response, cancellationToken);
    }
}
