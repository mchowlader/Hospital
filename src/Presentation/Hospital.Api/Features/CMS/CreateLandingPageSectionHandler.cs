using FluentValidation;
using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.LandingPage;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.CMS;

public class CreateLandingPageSectionHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<CreateLandingPageSectionRequest> _validator;

    public CreateLandingPageSectionHandler(AppDbContext dbContext, IValidator<CreateLandingPageSectionRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<long>> HandleAsync(CreateLandingPageSectionRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }

        var section = new LandingPageSection
        {
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            ImageUrl = request.ImageUrl?.Trim(),
            DisplayOrder = request.DisplayOrder,
            SectionType = request.SectionType.Trim(),
            IsVisible = request.IsVisible
        };

        await _dbContext.LandingPageSections.AddAsync(section, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(section.Id);
    }
}
