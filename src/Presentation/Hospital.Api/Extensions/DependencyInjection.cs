using Hospital.Api.Common;
using Hospital.Domain.Common;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Linq;
using System.Text;
using FluentValidation;

namespace Hospital.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add OpenAPI (Scalar) Registration
        services.AddOpenApi();

        // CORS Policy Configuration (for Blazor Client)
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Register DbContext and HttpContextAccessor
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMemoryCache();

        // Configure JWT Authentication
        var jwtKey = configuration["Jwt:Key"] ?? "super_secret_key_for_development_hospital_management_123456";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "Hospital.Api";
        var jwtAudience = configuration["Jwt:Audience"] ?? "Hospital.Client";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        // OpenTelemetry Tracing & Metrics Configuration
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .AddSource("Hospital.Api")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Hospital.Api"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource("Npgsql")
                .AddConsoleExporter()
                .AddOtlpExporter(opt => opt.Endpoint = new Uri("http://localhost:4317")))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(opt => opt.Endpoint = new Uri("http://localhost:4317")));

        // Automatically Register all Handler classes via Assembly Scanning
        var handlerTypes = typeof(DependencyInjection).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Handler"));

        foreach (var type in handlerTypes)
        {
            services.AddScoped(type);
        }

        // Register all validators from the Shared project assembly
        services.AddValidatorsFromAssemblyContaining<Result>();

        return services;
    }
}
