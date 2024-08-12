using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Abstractions;
using BackendChallenge.CrossCutting.Common;
using BackendChallenge.CrossCutting.Endpoints;
using BackendChallenge.CrossCutting.Services;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BackendChallenge.Application.Delivery.UseCases;
public static class RegisterDeliveryman
{
    public record HandlerRequest(
        string Username, string Password, string Email,
        string Name, string Cnpj, DateTime Birthdate, string CnhNumber, CnhType CnhType)
        : ICommand<Response>;
    public record Response(Guid Id, string Name, string Cnpj, DateTime Birthdate, string CnhNumber, CnhType CnhType);

    public sealed class Validator : AbstractValidator<HandlerRequest>
    {
        public Validator()
        {
            RuleFor(r => r.Username).NotEmpty();
            RuleFor(r => r.Password).NotEmpty();
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
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
            app.MapPost("api/deliverymen", async (
                HandlerRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok();
            })
            .WithTags("Deliveryman");
        }
    }

    internal sealed class Handler(
        ApplicationDbContext context,
        IAccountService<Account> accountService) : ICommandHandler<HandlerRequest, Response>
    {
        public async Task<Result<Response>> Handle(HandlerRequest request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                UserName = request.Username,
                Email = request.Email,
            };

            var result = await accountService.CreateAccount(account, request.Password, Roles.Deliveryman);

            if (result is null)
                return Result.Failure<Response>(DomainErrors.FailedToCreateAccount);

            var deliveryman = Deliveryman.Create(
                account.Id,
                request.Name,
                request.Cnpj,
                request.Birthdate,
                request.CnhNumber,
                request.CnhType);

            try
            {
                await context.Deliveryman.AddAsync(deliveryman);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                await accountService.RemoveFromRoleAsync(account, Roles.Deliveryman);
                await accountService.DeleteAsync(account);
                return Result.Failure<Response>(DomainErrors.FailedToCreate);
            }

            return deliveryman.Adapt<Response>();
        }
    }
}
