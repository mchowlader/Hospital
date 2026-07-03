using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class Permission : BaseEntity
{
    public string Key { get; set; } = default!;
    public string? Description { get; set; }
}
