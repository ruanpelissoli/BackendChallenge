using BackendChallenge.Application.Accounts;
using BackendChallenge.Application.Bikes.Events;
using BackendChallenge.CrossCutting.Endpoints;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BackendChallenge.Application.Bikes.UseCases;
public static class RegisterBike
{
    public record Request(int Year, string Model, string LicensePlate);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Year).NotEmpty();
            RuleFor(r => r.Model).NotEmpty();
            RuleFor(r => r.LicensePlate).NotEmpty();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("bikes", Handler)
               .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Admin })
               .WithTags("Bikes");
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] Request request,
        IPublishEndpoint _publisher,
        IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        await _publisher.Publish<BikeRegisteredEvent>(new(request.Year, request.Model, request.LicensePlate));

        return Results.Ok();
    }
}
