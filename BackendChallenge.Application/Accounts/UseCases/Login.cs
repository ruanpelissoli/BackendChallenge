using BackendChallenge.CrossCutting.Abstractions;
using BackendChallenge.CrossCutting.Common;
using BackendChallenge.CrossCutting.Endpoints;
using BackendChallenge.CrossCutting.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendChallenge.Application.Accounts.UseCases;
public static class Login
{
    public record HandlerRequest(string Username, string Password) : ICommand<Response>;
    public record Response(string Token, DateTime Expiration);

    public sealed class Validator : AbstractValidator<HandlerRequest>
    {
        public Validator()
        {
            RuleFor(r => r.Username).NotEmpty();
            RuleFor(r => r.Password).NotEmpty();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/account/login", async (
                HandlerRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result);
            })
            .WithTags("Account");
        }
    }

    internal sealed class Handler(
        IAccountService<Account> accountService,
        IConfiguration configuration) : ICommandHandler<HandlerRequest, Response>
    {
        public async Task<Result<Response>> Handle(HandlerRequest request, CancellationToken cancellationToken)
        {
            var account = await accountService.FindByNameAsync(request.Username);

            if (account == null || !await accountService.CheckPasswordAsync(account, request.Password))
                return Result.Failure<Response>(DomainErrors.LoginError);

            var userRoles = await accountService.GetRolesAsync(account);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, account.UserName!),
                new(ClaimTypes.NameIdentifier, account.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                expires: DateTime.Now.AddYears(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Result.Success(new Response(
                new JwtSecurityTokenHandler().WriteToken(token),
                token.ValidTo));
        }
    }
}
