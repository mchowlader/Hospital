using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Hospital.Client.Services;

/// <summary>
/// Dynamically resolves authorization policies using naming convention:
///   Policy "DepartmentView" → RequireClaim("Permission", "Department.View")
///   Policy "DoctorCreate"    → RequireClaim("Permission", "Doctor.Create")
///
/// Any policy not matching the convention falls back to explicitly registered policies
/// (e.g., "AdminMenu", "DefinitionsMenu").
/// </summary>
public class DynamicPermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private static readonly string[] KnownActions = ["View", "Create", "Edit", "Delete", "Export"];

    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public DynamicPermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallback = new DefaultAuthorizationPolicyProvider(options);
    }

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Try to match convention: {EntityName}{Action}
        // e.g., "DepartmentView" → entity="Department", action="View" → claim "Department.View"
        foreach (var action in KnownActions)
        {
            if (policyName.EndsWith(action, StringComparison.OrdinalIgnoreCase))
            {
                var entityName = policyName[..^action.Length];
                if (!string.IsNullOrWhiteSpace(entityName))
                {
                    return new AuthorizationPolicyBuilder()
                        .RequireClaim("Permission", $"{entityName}.{action}")
                        .Build();
                }
            }
        }

        // Fallback to explicitly registered policies (AdminMenu, etc.)
        return await _fallback.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => _fallback.GetFallbackPolicyAsync();
}
