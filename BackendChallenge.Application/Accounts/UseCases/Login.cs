using BackendChallenge.CrossCutting.Endpoints;
using BackendChallenge.CrossCutting.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendChallenge.Application.Accounts.UseCases;
public static class Login
{
    public record Request(string Username, string Password);

    public sealed class Validator : AbstractValidator<Request>
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
            app.MapPost("api/account/login", Handler)
               .WithTags("Account");
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] Request request,
        IAccountService<Account> accountService,
        IConfiguration configuration,
        IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var account = await accountService.FindByNameAsync(request.Username);
        if (account != null && await accountService.CheckPasswordAsync(account, request.Password))
        {
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

            return Results.Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Results.Unauthorized();
    }
}
