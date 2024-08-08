using Microsoft.AspNetCore.Routing;

namespace BackendChallenge.CrossCutting.Endpoints;
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
