namespace Hospital.Shared.DTOs.Membership;

public class AuthResponse
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Mobile { get; set; } = default!;
}
