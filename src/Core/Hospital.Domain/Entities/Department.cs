using System.Collections.Generic;
using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    public ICollection<DepartmentTerm> DepartmentTerms { get; set; } = new List<DepartmentTerm>();
}
