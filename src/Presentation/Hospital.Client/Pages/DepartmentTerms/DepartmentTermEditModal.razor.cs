using Hospital.Client.Services;
using Hospital.Shared.DTOs.Departments;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Client.Pages.DepartmentTerms;

public partial class DepartmentTermEditModal : ComponentBase
{
    [Inject]
    private DepartmentTermServiceClient TermService { get; set; } = default!;

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public DepartmentTermDto? Term { get; set; }

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    protected UpdateDepartmentTermRequest FormModel { get; set; } = new();
    protected string? ModalErrorMessage { get; set; }
    protected bool IsSaving { get; set; }

    protected override void OnParametersSet()
    {
        if (Show && Term != null && (FormModel == null || FormModel.Id != Term.Id))
        {
            ResetForm();
        }
    }

    private void ResetForm()
    {
        if (Term != null)
        {
            FormModel = new UpdateDepartmentTermRequest
            {
                Id = Term.Id,
                DepartmentId = Term.DepartmentId,
                Name = Term.Name,
                Description = Term.Description,
                BaseCost = Term.BaseCost
            };
        }
        ModalErrorMessage = null;
        IsSaving = false;
    }

    protected async Task CloseModal()
    {
        await OnClose.InvokeAsync();
    }

    protected async Task SaveTerm()
    {
        if (Term == null) return;

        IsSaving = true;
        ModalErrorMessage = null;
        try
        {
            var result = await TermService.UpdateDepartmentTermAsync(Term.Id, FormModel);
            if (result.IsSuccess)
            {
                var termName = FormModel.Name;
                await OnSuccess.InvokeAsync(termName);
            }
            else
            {
                ModalErrorMessage = result.Error?.Description ?? "Failed to update treatment term.";
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
