using System.Collections.Generic;

namespace Hospital.Shared.DTOs.Membership;

public class UserDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Mobile { get; set; } = default!;
    public bool IsActive { get; set; }
    public List<string> RoleNames { get; set; } = new();
}

public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string> PermissionKeys { get; set; } = new();
}
