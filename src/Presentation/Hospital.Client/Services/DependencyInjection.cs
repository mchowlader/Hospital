using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Client.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Authorization options
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("AdminMenu", policy => policy.RequireAssertion(ctx =>
                ctx.User.HasClaim(c => c.Type == "Permission" &&
                    (c.Value == "User.View" || c.Value == "Role.View"))));

            options.AddPolicy("DefinitionsMenu", policy => policy.RequireAssertion(ctx =>
                ctx.User.HasClaim(c => c.Type == "Permission" &&
                    (c.Value == "Department.View" || c.Value == "DepartmentTerm.View" || c.Value == "Doctor.View"))));

            options.AddPolicy("OperationsMenu", policy => policy.RequireAssertion(ctx =>
                ctx.User.HasClaim(c => c.Type == "Permission" &&
                    (c.Value == "Patient.View" || c.Value == "Appointment.View" || 
                     c.Value == "TreatmentHistory.View" || c.Value == "Billing.View"))));
        });

        // Register the convention-based policy provider.
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();

        // Register Infrastructure Services
        services.AddScoped<LocalStorageService>();
        services.AddScoped<JwtAuthorizationMessageHandler>();

        // Register HttpClient configured to use the authorization handler
        services.AddScoped(sp =>
        {
            var authHandler = sp.GetRequiredService<JwtAuthorizationMessageHandler>();
            authHandler.InnerHandler = new HttpClientHandler();
            
            var apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:11290/";
            
            return new HttpClient(authHandler)
            {
                BaseAddress = new Uri(apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(100)
            };
        });

        services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

        // Register Application Service Clients
        services.AddScoped<AuthServiceClient>();
        services.AddScoped<DepartmentServiceClient>();
        services.AddScoped<DepartmentTermServiceClient>();
        services.AddScoped<LandingPageServiceClient>();
        services.AddScoped<DoctorServiceClient>();
        services.AddScoped<PatientServiceClient>();

        return services;
    }
}
