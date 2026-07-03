using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class RolePermission : BaseEntity
{
    public long RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public long PermissionId { get; set; }
    public Permission Permission { get; set; } = default!;
}
