using System.Collections.Generic;
using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
