using BackendChallenge.CrossCutting.Endpoints;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Application.Bikes.UseCases;
public static class UpdateBikeLicensePlate
{
    public record Request(string LicensePlate);
    public record Response(Guid Id, int Year, string Model, string LicensePlate);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("bikes/{id}", Handler)
               .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
               .WithTags("Bikes");
        }
    }

    public static async Task<IResult> Handler(
       Guid id,
       Request request,
       ApplicationDbContext context)
    {
        var bike = await context.Bikes.FirstOrDefaultAsync(b => b.Id == id);

        if (bike is null)
            return Results.NotFound();

        bike.UpdateLicensePlate(request.LicensePlate);

        await context.SaveChangesAsync();

        return Results.Ok(bike.Adapt<Response>());
    }
}
