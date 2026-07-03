using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Hospital.Shared.DTOs.Membership;
using Hospital.Client.Services;

namespace Hospital.Client.Pages.Auth;

public partial class Login : ComponentBase
{
    [Inject]
    public AuthServiceClient AuthServiceClient { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    protected LoginRequest Model { get; set; } = new();
    protected string? ErrorMessage { get; set; }
    protected bool IsLoading { get; set; }

    protected async Task HandleLogin()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var result = await AuthServiceClient.LoginAsync(Model);
            if (result.IsSuccess)
            {
                // After successful login, redirect to dashboard (e.g. departments list page)
                NavigationManager.NavigateTo("departments");
            }
            else
            {
                ErrorMessage = result.Error?.Description ?? "Invalid username or password.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An unexpected error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
