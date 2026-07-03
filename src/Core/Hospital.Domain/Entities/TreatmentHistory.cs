using System;
using System.Collections.Generic;
using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class TreatmentHistory : BaseEntity
{
    public long PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public long DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    public long DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    public string Diagnosis { get; set; } = default!;
    public DateTime TreatmentDate { get; set; }

    public ICollection<TreatmentDetail> TreatmentDetails { get; set; } = new List<TreatmentDetail>();
}
