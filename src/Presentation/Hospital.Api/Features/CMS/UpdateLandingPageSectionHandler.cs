using FluentValidation;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.LandingPage;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.CMS;

public class UpdateLandingPageSectionHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<UpdateLandingPageSectionRequest> _validator;

    public UpdateLandingPageSectionHandler(AppDbContext dbContext, IValidator<UpdateLandingPageSectionRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateLandingPageSectionRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }

        var section = await _dbContext.LandingPageSections
            .SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (section == null)
        {
            return Result.Failure(Error.NotFound("Landing page section not found."));
        }

        section.Title = request.Title.Trim();
        section.Content = request.Content.Trim();
        section.ImageUrl = request.ImageUrl?.Trim();
        section.DisplayOrder = request.DisplayOrder;
        section.SectionType = request.SectionType.Trim();
        section.IsVisible = request.IsVisible;

        _dbContext.LandingPageSections.Update(section);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
