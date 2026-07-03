using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class UserPermission : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public long PermissionId { get; set; }
    public Permission Permission { get; set; } = default!;

    public bool IsAllowed { get; set; }
}
