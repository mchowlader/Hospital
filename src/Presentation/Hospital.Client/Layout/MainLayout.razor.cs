using Hospital.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Hospital.Client.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private AuthServiceClient AuthServiceClient { get; set; } = default!;

    private bool _isMobileMenuOpen;

    private void ToggleMobileMenu()
    {
        _isMobileMenuOpen = !_isMobileMenuOpen;
    }

    protected string GetInitials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "U";
        var parts = name.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
        return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
    }

    private async Task HandleLogout()
    {
        await AuthServiceClient.LogoutAsync();
        NavigationManager.NavigateTo("login");
    }
}
