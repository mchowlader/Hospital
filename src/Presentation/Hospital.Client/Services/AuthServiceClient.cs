using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Membership;

namespace Hospital.Client.Services;

public class AuthServiceClient : BaseServiceClient
{
    private const string BaseUrl = "api/auth";

    public AuthServiceClient(
        HttpClient httpClient, 
        LocalStorageService localStorage, 
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider)
        : base(httpClient, localStorage, authStateProvider)
    {
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/login", request, cancellationToken);
        var result = await HandleResponseAsync<AuthResponse>(response, cancellationToken);

        if (result.IsSuccess && result.Value != null)
        {
            await LocalStorage.SetItemAsync("authToken", result.Value.Token);
            await LocalStorage.SetItemAsync("refreshToken", result.Value.RefreshToken);

            var meResult = await GetMeAsync(cancellationToken);
            if (meResult.IsSuccess && meResult.Value != null)
            {
                AuthStateProvider.MarkUserAsAuthenticated(meResult.Value);
            }
        }

        return result;
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/refresh", request, cancellationToken);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Result<AuthResponse>>(cancellationToken: cancellationToken);
            if (result != null && result.IsSuccess && result.Value != null)
            {
                await LocalStorage.SetItemAsync("authToken", result.Value.Token);
                await LocalStorage.SetItemAsync("refreshToken", result.Value.RefreshToken);
                return result;
            }
            return Result<AuthResponse>.Failure(Error.Unexpected("Failed to parse refresh response."));
        }

        var error = await ReadErrorFromResponseAsync(response, cancellationToken);
        return Result<AuthResponse>.Failure(error);
    }

    public async Task<Result<UserMeResponse>> GetMeAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync($"{BaseUrl}/me", cancellationToken);
        return await HandleResponseAsync<UserMeResponse>(response, cancellationToken);
    }

    public async Task<Result<long>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", request, cancellationToken);
        return await HandleResponseAsync<long>(response, cancellationToken);
    }
}
