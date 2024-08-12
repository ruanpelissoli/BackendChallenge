using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace BackendChallenge.Application.Rentals.UseCases;
public static class RentABike
{
    public record Response(Guid RentalId, DateOnly StartDate, DateOnly EndDate, decimal TotalValue, BikeResponse Bike);
    public record BikeResponse(Guid BikeId, string Model, int Year, string LicensePlate);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/rentals/bike/{bikeId}/plan/{planId}", Handler)
               .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Deliveryman })
               .WithTags("Rentals");
        }
    }

    public static async Task<IResult> Handler(
        Guid bikeId,
        Guid planId,
        IRepository repository,
        HttpContext httpContext)
    {
        var accountId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(accountId))
            return Results.Unauthorized();

        var deliveryman = await repository.GetDelivymanByAccountId(accountId);

        if (deliveryman is null)
            return Results.Unauthorized();

        if (deliveryman.CnhType != Delivery.CnhType.A)
            return Results.BadRequest("Only deliveryman with CNH type A can rent a bike.");

        var bike = await repository.GetBikeById(bikeId);

        if (bike is null)
            return Results.NotFound();

        var plan = await repository.GetPlanById(planId);

        if (plan is null)
            return Results.NotFound();

        var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var endDate = startDate.AddDays(plan.DurationInDays);

        if (await repository.IsBikeAvailableForRental(bikeId, startDate, endDate) is false)
            return Results.BadRequest("Bike is not available for the period.");

        var rental = Rental.Create(bikeId, deliveryman.Id, planId, plan.TotalValue, startDate, endDate);

        await repository.CreateRental(rental);
        await repository.Commit();

        var response = new Response(rental.Id, rental.StartDate, rental.EndDate, plan.TotalValue, new BikeResponse(bike.Id, bike.Model, bike.Year, bike.LicensePlate));

        return Results.Ok(response);
    }
}
