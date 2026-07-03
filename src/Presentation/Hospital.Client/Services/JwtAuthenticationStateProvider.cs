using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Hospital.Shared.DTOs.Membership;
using Hospital.Shared.Common;

namespace Hospital.Client.Services;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly LocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public JwtAuthenticationStateProvider(LocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync("authToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(Anonymous);
        }

        try
        {
            // Set the authorization header manually here just in case the message handler hasn't run yet or for the startup call
            var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                // Token invalid or expired, clear it
                await _localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(Anonymous);
            }

            var result = await response.Content.ReadFromJsonAsync<Result<UserMeResponse>>();
            if (result == null || !result.IsSuccess || result.Value == null)
            {
                await _localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(Anonymous);
            }

            var userMe = result.Value;
            var identity = CreateIdentityFromUserMe(userMe);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationState(principal);
        }
        catch (Exception)
        {
            // Network error or server offline: return anonymous is safer for complete protection.
            return new AuthenticationState(Anonymous);
        }
    }

    public void MarkUserAsAuthenticated(UserMeResponse userMe)
    {
        var identity = CreateIdentityFromUserMe(userMe);
        var principal = new ClaimsPrincipal(identity);
        var authState = Task.FromResult(new AuthenticationState(principal));
        NotifyAuthenticationStateChanged(authState);
    }

    public void MarkUserAsLoggedOut()
    {
        var authState = Task.FromResult(new AuthenticationState(Anonymous));
        NotifyAuthenticationStateChanged(authState);
    }

    private static ClaimsIdentity CreateIdentityFromUserMe(UserMeResponse userMe)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userMe.Id.ToString()),
            new(ClaimTypes.Name, userMe.Name),
            new("Username", userMe.Username),
            new(ClaimTypes.Email, userMe.Email),
            new("Mobile", userMe.Mobile)
        };

        if (userMe.DoctorId.HasValue)
        {
            claims.Add(new Claim("DoctorId", userMe.DoctorId.Value.ToString()));
        }

        if (userMe.PatientId.HasValue)
        {
            claims.Add(new Claim("PatientId", userMe.PatientId.Value.ToString()));
        }

        // Add Roles as standard Claims
        foreach (var role in userMe.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add Permissions as custom claim type "Permission"
        foreach (var permission in userMe.Permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }

        return new ClaimsIdentity(claims, "jwt");
    }
}
