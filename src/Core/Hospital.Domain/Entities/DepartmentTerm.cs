using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class DepartmentTerm : BaseEntity
{
    public long DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal BaseCost { get; set; }
}
