using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Shared.DTOs.Membership;

public class CreateRoleRequest
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Role name must be between 3 and 50 characters")]
    public string Name { get; set; } = default!;

    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }
}

public class UpdateRoleRequest
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Role name must be between 3 and 50 characters")]
    public string Name { get; set; } = default!;

    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }
}

public class AssignRolePermissionsRequest
{
    public long RoleId { get; set; }
    public List<string> PermissionKeys { get; set; } = new();
}

public class AssignUserRolesRequest
{
    public long UserId { get; set; }
    public List<long> RoleIds { get; set; } = new();
}

public class UserPermissionOverride
{
    public string PermissionKey { get; set; } = default!;
    public bool IsAllowed { get; set; }
}

public class AssignUserPermissionsRequest
{
    public long UserId { get; set; }
    public List<UserPermissionOverride> Overrides { get; set; } = new();
}

public class UserDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Mobile { get; set; } = default!;
    public bool IsActive { get; set; }
    public List<long> RoleIds { get; set; } = new();
    public List<UserPermissionOverride> PermissionOverrides { get; set; } = new();
}

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Mobile number is required")]
    [RegularExpression(@"^(?:\+88|88)?(01[3-9]\d{8})$", ErrorMessage = "Invalid Bangladeshi mobile number")]
    public string Mobile { get; set; } = default!;

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string? NewPassword { get; set; }

    public string? CurrentPassword { get; set; }
}

public class UpdateUserStatusRequest
{
    public long UserId { get; set; }
    public bool IsActive { get; set; }
}

public class ResetUserPasswordRequest
{
    public long UserId { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string NewPassword { get; set; } = default!;
}
