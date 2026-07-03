using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class TreatmentDetail : BaseEntity
{
    public long TreatmentHistoryId { get; set; }
    public TreatmentHistory TreatmentHistory { get; set; } = default!;

    public long DepartmentTermId { get; set; }
    public DepartmentTerm DepartmentTerm { get; set; } = default!;

    public int Quantity { get; set; } = 1;
    public decimal ActualCost { get; set; }
    public string? Notes { get; set; }
}
