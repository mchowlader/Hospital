using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.LandingPage;

namespace Hospital.Client.Services;

public class LandingPageServiceClient : BaseServiceClient
{
    private const string BaseUrl = "api/landing-page";

    public LandingPageServiceClient(
        HttpClient httpClient, 
        LocalStorageService localStorage, 
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider)
        : base(httpClient, localStorage, authStateProvider)
    {
    }

    public async Task<Result<List<LandingPageSectionDto>>> GetLandingPageSectionsAsync(
        bool includeHidden,
        CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}?includeHidden={includeHidden}";
        var response = await HttpClient.GetAsync(url, cancellationToken);
        return await HandleResponseAsync<List<LandingPageSectionDto>>(response, cancellationToken);
    }

    public async Task<Result<long>> CreateLandingPageSectionAsync(
        CreateLandingPageSectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        return await HandleResponseAsync<long>(response, cancellationToken);
    }

    public async Task<Result> UpdateLandingPageSectionAsync(
        long id,
        UpdateLandingPageSectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    public async Task<Result> DeleteLandingPageSectionAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }
}
