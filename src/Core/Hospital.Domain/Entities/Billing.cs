using System;
using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class Billing : BaseEntity
{
    public long PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public long? TreatmentHistoryId { get; set; }
    public TreatmentHistory? TreatmentHistory { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, PartiallyPaid
    public DateTime BillingDate { get; set; }
}
