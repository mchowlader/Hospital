using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.LandingPage;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Api.Features.CMS;

public class GetLandingPageSectionsHandler
{
    private readonly AppDbContext _dbContext;

    public GetLandingPageSectionsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<LandingPageSectionDto>>> HandleAsync(bool includeHidden, CancellationToken cancellationToken)
    {
        var query = _dbContext.LandingPageSections
            .AsNoTracking()
            .AsQueryable();

        if (!includeHidden)
        {
            query = query.Where(s => s.IsVisible);
        }

        var sections = await query
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new LandingPageSectionDto
            {
                Id = s.Id,
                Title = s.Title,
                Content = s.Content,
                ImageUrl = s.ImageUrl,
                DisplayOrder = s.DisplayOrder,
                SectionType = s.SectionType,
                IsVisible = s.IsVisible
            })
            .ToListAsync(cancellationToken);

        return Result<List<LandingPageSectionDto>>.Success(sections);
    }
}
