using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Endpoints;
using BackendChallenge.CrossCutting.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Application.Delivery.UseCases;
public static class UploadCnhImage
{
    public record Response(Guid Id, string ImageUrl);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/deliverymen/{id}/upload-cnh", Handler)
               .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Deliveryman })
               .DisableAntiforgery()
               .WithTags("Deliveryman");
        }
    }

    public static async Task<IResult> Handler(
       Guid id,
       [FromForm] IFormFile image,
       [FromServices] ApplicationDbContext context,
       [FromServices] IStorageService storageService)
    {

        if (image == null || (image.ContentType != "image/png" && image.ContentType != "image/bmp"))
        {
            return Results.BadRequest("Invalid image type. Only PNG and BMP are allowed.");
        }

        var deliveryman = await context.Deliveryman.FirstOrDefaultAsync(d => d.Id == id);

        if (deliveryman is null)
            return Results.NotFound();

        using var memoryStream = new MemoryStream();

        await image.CopyToAsync(memoryStream);
        var imageBytes = memoryStream.ToArray();
        var blobName = $"{id}/{image.FileName}";

        var imageUrl = await storageService.UploadImageAsync(imageBytes, blobName);

        deliveryman.UpdateCnhImageUrl(imageUrl);

        await context.SaveChangesAsync();

        var response = new Response(id, imageUrl);
        return Results.Ok(response);

    }
}
