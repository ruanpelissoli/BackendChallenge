using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Abstractions;
using BackendChallenge.CrossCutting.Common;
using BackendChallenge.CrossCutting.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Application.Bikes.UseCases;
public static class DeleteBike
{
    public record HandlerRequest(Guid BikeId) : ICommand;

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/bikes/{id}", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                HandlerRequest handlerRequest = new(id);

                var result = await sender.Send(handlerRequest, cancellationToken);

                if (result.IsFailure)
                    return Results.NotFound(result);

                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Admin })
            .WithTags("Bikes");
        }
    }

    internal sealed class Handler(ApplicationDbContext context) : ICommandHandler<HandlerRequest>
    {
        public async Task<Result> Handle(HandlerRequest request, CancellationToken cancellationToken)
        {
            var bike = await context.Bikes.FindAsync(request.BikeId);

            if (bike is null)
                return Result.Failure(DomainErrors.NotFound);

            var bikeHasRentalRecord = await context.Rentals.FirstOrDefaultAsync(r => r.BikeId == bike.Id);

            if (bikeHasRentalRecord is not null)
                return Result.Failure(DomainErrors.BikeHasRentalRecord);

            context.Remove(bike);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
