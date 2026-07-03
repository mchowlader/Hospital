using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hospital.Shared.DTOs.Membership;

namespace Hospital.Client.Services;

public class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly LocalStorageService _localStorage;
    private readonly IServiceProvider _serviceProvider;

    public JwtAuthorizationMessageHandler(LocalStorageService localStorage, IServiceProvider serviceProvider)
    {
        _localStorage = localStorage;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var relativePath = request.RequestUri?.AbsolutePath?.ToLower() ?? "";
        
        // Prevent intercepting token refresh, login, or registration endpoints
        if (relativePath.Contains("/api/auth/login") || 
            relativePath.Contains("/api/auth/register") || 
            relativePath.Contains("/api/auth/refresh"))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var token = await _localStorage.GetItemAsync("authToken");

        if (!string.IsNullOrWhiteSpace(token) && JwtTokenHelper.IsTokenExpiredOrNearExpiry(token, TimeSpan.FromSeconds(30)))
        {
            var refreshToken = await _localStorage.GetItemAsync("refreshToken");
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                try
                {
                    // Resolve AuthServiceClient dynamically to break the circular dependency loop in DI
                    var authService = _serviceProvider.GetRequiredService<AuthServiceClient>();
                    
                    var refreshResult = await authService.RefreshTokenAsync(new RefreshTokenRequest
                    {
                        AccessToken = token,
                        RefreshToken = refreshToken
                    }, cancellationToken);

                    if (refreshResult.IsSuccess && refreshResult.Value != null)
                    {
                        token = refreshResult.Value.Token;
                    }
                    else
                    {
                        await authService.LogoutAsync();
                        token = null;
                    }
                }
                catch
                {
                    // If refresh fails due to network error, let the original request proceed and fail with 401
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // If response is 403 Forbidden, permissions might have updated on the server. Try to refresh the token and retry.
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            var refreshToken = await _localStorage.GetItemAsync("refreshToken");
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                try
                {
                    var authService = _serviceProvider.GetRequiredService<AuthServiceClient>();
                    var currentToken = await _localStorage.GetItemAsync("authToken");
                    if (!string.IsNullOrWhiteSpace(currentToken))
                    {
                        var refreshResult = await authService.RefreshTokenAsync(new RefreshTokenRequest
                        {
                            AccessToken = currentToken,
                            RefreshToken = refreshToken
                        }, cancellationToken);

                        if (refreshResult.IsSuccess && refreshResult.Value != null)
                        {
                            var newToken = refreshResult.Value.Token;
                            
                            // Clone request, attach the new token, and retry the request
                            var clonedRequest = await CloneHttpRequestMessageAsync(request);
                            clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                            
                            response.Dispose();
                            return await base.SendAsync(clonedRequest, cancellationToken);
                        }
                    }
                }
                catch
                {
                    // Ignore and fallback to original 403 response
                }
            }
        }

        return response;
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri);
        
        // Copy headers
        foreach (var header in req.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        
        // Copy properties/options
        foreach (var option in req.Options)
        {
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);
        }

        // Copy content
        if (req.Content != null)
        {
            var ms = new System.IO.MemoryStream();
            await req.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);
            
            foreach (var h in req.Content.Headers)
            {
                clone.Content.Headers.Add(h.Key, h.Value);
            }
        }

        return clone;
    }
}
