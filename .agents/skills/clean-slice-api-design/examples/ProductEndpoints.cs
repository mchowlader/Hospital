using System.Threading;
using System.Threading.Tasks;
using Hospital.Api.Common;
using Hospital.Api.Extensions;
using Hospital.Shared.DTOs.Definitions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Api.Features.Definitions.Products;

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products Management");

        group.MapGet("/", GetProductsAsync)
            .WithName("GetProducts")
            .RequirePermission("Product.View");

        group.MapGet("/{id:long}", GetProductByIdAsync)
            .WithName("GetProductById")
            .RequirePermission("Product.View");

        group.MapGet("/export", ExportProductsAsync)
            .WithName("ExportProducts")
            .RequirePermission("Product.View");

        group.MapPost("/", CreateProductAsync)
            .WithName("CreateProduct")
            .RequirePermission("Product.Create");

        group.MapPut("/{id:long}", UpdateProductAsync)
            .WithName("UpdateProduct")
            .RequirePermission("Product.Edit");

        group.MapDelete("/{id:long}", DeleteProductAsync)
            .WithName("DeleteProduct")
            .RequirePermission("Product.Delete");
    }

    private static async Task<IResult> GetProductsAsync(
        int? page,
        int? size,
        string? search,
        long? categoryId,
        long? brandId,
        GetProductsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(page ?? 1, size ?? 10, search, categoryId, brandId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetProductByIdAsync(
        long id,
        GetProductByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> CreateProductAsync(
        CreateProductRequest request,
        CreateProductHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result)
            : result.ToProblemDetails();
    }
}
