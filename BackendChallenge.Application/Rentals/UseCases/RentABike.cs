using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Abstractions;
using BackendChallenge.CrossCutting.Common;
using BackendChallenge.CrossCutting.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace BackendChallenge.Application.Rentals.UseCases;
public static class RentABike
{
    public record HandlerRequest(Guid BikeId, Guid PlanId, string AccountId) : ICommand<Response>;
    public record Response(Guid RentalId, DateOnly StartDate, DateOnly EndDate, decimal TotalValue, BikeResponse Bike);
    public record BikeResponse(Guid BikeId, string Model, int Year, string LicensePlate);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/rentals/bike/{bikeId}/plan/{planId}", async (
                Guid bikeId,
                Guid planId,
                ISender sender,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var accountId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(accountId))
                    return Results.Unauthorized();

                var handlerRequest = new HandlerRequest(bikeId, planId, accountId);

                var result = await sender.Send(handlerRequest, cancellationToken);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result);

            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Deliveryman })
            .WithTags("Rentals");
        }
    }

    internal sealed class Handler(
        IRepository repository) : ICommandHandler<HandlerRequest, Response>
    {
        public async Task<Result<Response>> Handle(HandlerRequest request, CancellationToken cancellationToken)
        {
            var deliveryman = await repository.GetDelivymanByAccountId(request.AccountId);

            if (deliveryman is null)
                return Result.Failure<Response>(DomainErrors.NotFound);

            if (deliveryman.CnhType != Delivery.CnhType.A)
                return Result.Failure<Response>(DomainErrors.InvalidCnhType);

            var bike = await repository.GetBikeById(request.BikeId);

            if (bike is null)
                return Result.Failure<Response>(DomainErrors.NotFound);

            var plan = await repository.GetPlanById(request.PlanId);

            if (plan is null)
                return Result.Failure<Response>(DomainErrors.NotFound);

            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = startDate.AddDays(plan.DurationInDays);

            if (await repository.IsBikeAvailableForRental(request.BikeId, startDate, endDate) is false)
                return Result.Failure<Response>(DomainErrors.BikeNotAvailable);

            var rental = Rental.Create(request.BikeId, deliveryman.Id, request.PlanId, plan.TotalValue, startDate, endDate);

            await repository.CreateRental(rental);
            await repository.Commit();

            var response = new Response(
                rental.Id,
                rental.StartDate,
                rental.EndDate,
                plan.TotalValue,
                new BikeResponse(bike.Id, bike.Model, bike.Year, bike.LicensePlate));

            return response;
        }
    }
}
