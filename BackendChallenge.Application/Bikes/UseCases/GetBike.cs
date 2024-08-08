using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Endpoints;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Application.Bikes.UseCases;
public static class GetBike
{
    public record Response(Guid Id, int Year, string Model, string LicensePlate);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("bikes", Handler)
               .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Admin })
               .WithTags("Bikes");
        }
    }

    public static async Task<IResult> Handler(
       [FromQuery] string? licensePlate,
       ApplicationDbContext context)
    {
        var query = context.Bikes.AsQueryable();

        if (!string.IsNullOrEmpty(licensePlate))
            query = query.Where(b => b.LicensePlate.Contains(licensePlate));

        var result = await query.ToListAsync();

        return Results.Ok(result.Adapt<IEnumerable<Response>>());
    }
}
