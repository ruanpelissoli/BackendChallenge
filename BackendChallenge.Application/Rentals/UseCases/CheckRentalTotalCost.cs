using BackendChallenge.Application.Accounts;
using BackendChallenge.Application.Rentals.Services;
using BackendChallenge.CrossCutting.Abstractions;
using BackendChallenge.CrossCutting.Common;
using BackendChallenge.CrossCutting.Endpoints;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BackendChallenge.Application.Rentals.UseCases;
public static class CheckRentalTotalCost
{
    public record Request(DateTime ReturnDate);
    public record HandlerRequest(Guid RentalId, DateTime ReturnDate) : ICommand<Response>;
    public record Response(
        decimal DaysUsedCost,
        decimal FineCostPerDaysRemaining,
        decimal CostPerAdditionalDays,
        decimal TotalCost);

    public class HandlerRequestValidator : AbstractValidator<HandlerRequest>
    {
        public HandlerRequestValidator()
        {
            RuleFor(r => r.RentalId).NotEmpty();
            RuleFor(r => r.ReturnDate).NotEmpty();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/rentals/{rentalId}/return",
                async (Guid rentalId, Request request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var handlerRequest = new HandlerRequest(rentalId, request.ReturnDate);

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
        IBikeRentalReturnCalculation bikeRentalReturnCalculation,
        IRepository repository) : ICommandHandler<HandlerRequest, Response>
    {
        public async Task<Result<Response>> Handle(HandlerRequest request, CancellationToken cancellationToken)
        {
            var rental = await repository.GetRentalById(request.RentalId);

            if (rental is null)
                return Result.Failure<Response>(DomainErrors.NotFound);

            var returnDate = DateOnly.FromDateTime(request.ReturnDate);

            var calculation = bikeRentalReturnCalculation.CalculateTotalCost(returnDate, rental);

            return calculation.Adapt<Response>();
        }
    }
}
