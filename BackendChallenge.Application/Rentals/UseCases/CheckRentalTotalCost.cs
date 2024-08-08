using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Application.Rentals.UseCases;
public static class CheckRentalTotalCost
{
    public record Request(DateTime ReturnDate);

    public record Response(
        decimal DaysUsedCost,
        decimal FineCostPerDaysRemaining,
        decimal CostPerAdditionalDays,
        decimal TotalCost);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("rental/{rentalId}/return", Handler)
               .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Deliveryman })
               .WithTags("Rentals");
        }
    }

    public static async Task<IResult> Handler(
        Guid rentalId,
        [FromBody] Request request,
        ApplicationDbContext context)
    {
        var rental = await context.Rentals
            .Include(r => r.Plan)
            .FirstOrDefaultAsync(r => r.Id == rentalId);

        if (rental is null)
            return Results.NotFound();

        var returnDate = DateOnly.FromDateTime(request.ReturnDate);

        if (returnDate < rental.EndDate)
        {
            var remainingDaysToFine = rental.EndDate.DayNumber - returnDate.DayNumber;
            var daysUsedToCharge = returnDate.DayNumber - rental.StartDate.DayNumber;

            var daysUsedCost = daysUsedToCharge * rental.Plan.CostPerDay;
            var fineCostPerDaysRemaining = (remainingDaysToFine * rental.Plan.CostPerDay) * (rental.Plan.FineCostPercentagePerDay / 100);
            var costPerAdditionalDays = 0M;

            var totalCost = daysUsedCost + fineCostPerDaysRemaining + costPerAdditionalDays;

            return Results.Ok(new Response(daysUsedCost, fineCostPerDaysRemaining, costPerAdditionalDays, totalCost));
        }

        {
            var daysUsedToCharge = rental.Plan.DurationInDays;
            var additionalDays = returnDate.DayNumber - rental.EndDate.DayNumber;

            var daysUsedCost = daysUsedToCharge * rental.Plan.CostPerDay;
            var fineCostPerDaysRemaining = 0M;
            var costPerAdditionalDays = additionalDays * 50M;

            var totalCost = daysUsedCost + fineCostPerDaysRemaining + costPerAdditionalDays;

            return Results.Ok(new Response(daysUsedCost, fineCostPerDaysRemaining, costPerAdditionalDays, totalCost));
        }
    }
}
