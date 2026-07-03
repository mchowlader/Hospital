using Hospital.Client.Services;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.DepartmentTerms;

public partial class DepartmentTermCreateModal : ComponentBase
{
    [Inject]
    private DepartmentTermServiceClient TermService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    protected CreateDepartmentTermRequest FormModel { get; set; } = new();
    protected string? ModalErrorMessage { get; set; }
    protected bool IsSaving { get; set; }

    protected override void OnParametersSet()
    {
        if (Show && FormModel.Name == null)
        {
            ResetForm();
        }
    }

    private void ResetForm()
    {
        FormModel = new CreateDepartmentTermRequest { DepartmentId = 0, BaseCost = 0 };
        ModalErrorMessage = null;
        IsSaving = false;
    }

    protected async Task CloseModal()
    {
        ResetForm();
        await OnClose.InvokeAsync();
    }

    protected async Task SaveTerm()
    {
        IsSaving = true;
        ModalErrorMessage = null;
        try
        {
            var result = await TermService.CreateDepartmentTermAsync(FormModel);
            if (result.IsSuccess)
            {
                var termName = FormModel.Name;
                ResetForm();
                await OnSuccess.InvokeAsync(termName);
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to create treatment term.";
            }
        }
        catch (Exception ex)
        {
            ModalErrorMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }
}
