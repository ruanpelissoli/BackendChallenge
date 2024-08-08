using BackendChallenge.CrossCutting.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Application.Bikes.UseCases;
public static class DeleteBike
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("bikes/{id}", Handler)
               .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
               .WithTags("Bikes");
        }
    }

    public static async Task<IResult> Handler(
       Guid id,
       ApplicationDbContext context)
    {
        var bike = await context.Bikes.FirstOrDefaultAsync(b => b.Id == id);

        if (bike is null)
            return Results.NotFound();

        context.Remove(bike);
        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
