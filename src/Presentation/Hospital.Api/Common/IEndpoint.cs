using Microsoft.AspNetCore.Routing;

namespace Hospital.Api.Common;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
