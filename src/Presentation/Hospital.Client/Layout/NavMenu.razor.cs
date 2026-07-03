using Microsoft.AspNetCore.Components;

namespace Hospital.Client.Layout;

public partial class NavMenu : ComponentBase
{
    [Parameter]
    public bool IsMobile { get; set; }

    protected bool IsDefinitionsSubmenuExpanded { get; set; } = false;

    protected void ToggleDefinitionsSubmenu()
    {
        IsDefinitionsSubmenuExpanded = !IsDefinitionsSubmenuExpanded;
    }
}
