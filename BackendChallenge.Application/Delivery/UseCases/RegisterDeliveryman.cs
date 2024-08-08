using BackendChallenge.CrossCutting.Endpoints;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BackendChallenge.Application.Delivery.UseCases;
public static class RegisterDeliveryman
{
    public record Request(string Name, string Cnpj, DateTime Birthdate, string CnhNumber, CnhType CnhType);
    public record Response(Guid Id, string Name, string Cnpj, DateTime Birthdate, string CnhNumber, CnhType CnhType);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.Cnpj).NotEmpty();
            RuleFor(r => r.Birthdate)
                .NotEmpty()
                .Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old.");
            RuleFor(r => r.CnhNumber).NotEmpty();
            RuleFor(r => r.CnhType)
                .NotEmpty()
                .IsInEnum();
        }

        private bool BeAtLeast18YearsOld(DateTime birthdate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthdate.Year;
            if (birthdate.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("deliveryman", Handler)
               .WithTags("Deliveryman");
        }
    }

    public static async Task<IResult> Handler(
        Request request,
        ApplicationDbContext context,
        IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var deliveryman = Deliveryman.Create(request.Name, request.Cnpj, request.Birthdate, request.CnhNumber, request.CnhType);

        await context.Deliveryman.AddAsync(deliveryman);
        await context.SaveChangesAsync();

        return Results.Ok(deliveryman.Adapt<Response>());
    }
}
