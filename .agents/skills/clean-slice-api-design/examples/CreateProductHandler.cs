using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Hospital.Shared.Common;
using Hospital.Shared.DTOs.Definitions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace Hospital.Api.Features.Definitions.Products;

public class CreateProductHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<CreateProductRequest> _validator;

    public CreateProductHandler(AppDbContext dbContext, IValidator<CreateProductRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<long>> HandleAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(Error.Validation(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }

        var trimmedStyleCode = request.StyleCode.Trim();

        // Check unique constraint on StyleCode
        var styleCodeExists = await _dbContext.Products
            .AnyAsync(p => p.StyleCode.ToLower() == trimmedStyleCode.ToLower(), cancellationToken);
        if (styleCodeExists)
        {
            return Result<long>.Failure(Error.Conflict("Product Style Code already exists."));
        }

        var product = new Product
        {
            StyleCode = trimmedStyleCode,
            Name = request.Name.Trim(),
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            CostPrice = request.CostPrice,
            SalePrice = request.SalePrice,
            IsActive = request.IsActive,
            IsDeleted = request.IsDeleted
        };

        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(product.Id);
    }
}
