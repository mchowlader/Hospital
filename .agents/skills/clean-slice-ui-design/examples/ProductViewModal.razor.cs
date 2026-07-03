using Hospital.Shared.DTOs.Definitions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.Definitions;

public partial class ProductViewModal : ComponentBase
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public ProductAdminDto? Product { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    protected async Task CloseModal()
    {
        await OnClose.InvokeAsync();
    }
}
