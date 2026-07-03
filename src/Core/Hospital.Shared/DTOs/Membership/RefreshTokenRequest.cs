namespace Hospital.Shared.DTOs.Membership;

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
