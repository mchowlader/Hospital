using System.Collections.Generic;

namespace Hospital.Shared.DTOs.Membership;

public class UserMeResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Mobile { get; set; } = default!;
    public long? DoctorId { get; set; }
    public long? PatientId { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}
