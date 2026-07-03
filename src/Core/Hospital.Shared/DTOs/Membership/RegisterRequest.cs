using System.ComponentModel.DataAnnotations;

namespace Hospital.Shared.DTOs.Membership;

public class RegisterRequest
{
    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; } = default!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Mobile number is required")]
    [RegularExpression(@"^(?:\+88|88)?(01[3-9]\d{8})$", ErrorMessage = "Invalid Bangladeshi mobile number")]
    public string Mobile { get; set; } = default!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = default!;
}
