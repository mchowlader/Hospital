using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class Doctor : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Specialization { get; set; } = default!;

    public long DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    public long? UserId { get; set; }
    public User? User { get; set; }
}
