using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class UserRole : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public long RoleId { get; set; }
    public Role Role { get; set; } = default!;
}
