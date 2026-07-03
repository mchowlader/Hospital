using System;
using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class Appointment : BaseEntity
{
    public long PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public long DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    public long DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled
    public string? Notes { get; set; }
}
