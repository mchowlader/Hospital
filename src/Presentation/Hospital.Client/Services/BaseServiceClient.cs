using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Hospital.Shared.Common;

namespace Hospital.Client.Services;

public abstract class BaseServiceClient
{
    protected readonly HttpClient HttpClient;
    protected readonly LocalStorageService LocalStorage;
    protected readonly JwtAuthenticationStateProvider AuthStateProvider;

    protected BaseServiceClient(
        HttpClient httpClient,
        LocalStorageService localStorage,
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider)
    {
        HttpClient = httpClient;
        LocalStorage = localStorage;
        AuthStateProvider = (JwtAuthenticationStateProvider)authStateProvider;
    }

    protected async Task<Result<T>> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Result<T>>(cancellationToken: cancellationToken);
            return result ?? Result<T>.Failure(Error.Unexpected("Failed to parse response."));
        }

        var error = await ReadErrorFromResponseAsync(response, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || 
            error.Type == ErrorType.Unauthorized)
        {
            await LogoutAsync();
        }

        return Result<T>.Failure(error);
    }

    protected async Task<Result> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken: cancellationToken);
            return result ?? Result.Failure(Error.Unexpected("Failed to parse response."));
        }

        var error = await ReadErrorFromResponseAsync(response, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || 
            error.Type == ErrorType.Unauthorized)
        {
            await LogoutAsync();
        }

        return Result.Failure(error);
    }

    public async Task LogoutAsync()
    {
        await LocalStorage.RemoveItemAsync("authToken");
        await LocalStorage.RemoveItemAsync("refreshToken");
        AuthStateProvider.MarkUserAsLoggedOut();
    }

    protected async Task<Error> ReadErrorFromResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(cancellationToken: cancellationToken);
            if (problem != null)
            {
                var type = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => ErrorType.NotFound,
                    System.Net.HttpStatusCode.BadRequest => ErrorType.Validation,
                    System.Net.HttpStatusCode.Conflict => ErrorType.Conflict,
                    System.Net.HttpStatusCode.Unauthorized => ErrorType.Unauthorized,
                    _ => ErrorType.Failure
                };
                return new Error(problem.Title ?? "Error", problem.Detail ?? "An error occurred.", type);
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => Error.Unauthorized("Unauthorized access."),
            System.Net.HttpStatusCode.Forbidden => new Error("Forbidden", "Forbidden access.", ErrorType.Failure),
            System.Net.HttpStatusCode.NotFound => Error.NotFound("Resource not found."),
            _ => Error.Unexpected($"Request failed with status code {response.StatusCode}.")
        };
    }

    private class ProblemDetailsResponse
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
    }
}
