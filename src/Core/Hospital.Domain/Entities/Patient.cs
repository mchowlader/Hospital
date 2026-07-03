using System;
using Hospital.Domain.Common;

namespace Hospital.Domain.Entities;

public class Patient : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = default!;
    public string? MedicalHistorySummary { get; set; }

    public long? UserId { get; set; }
    public User? User { get; set; }
}
