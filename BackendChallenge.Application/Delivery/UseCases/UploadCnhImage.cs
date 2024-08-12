using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Abstractions;
using BackendChallenge.CrossCutting.Common;
using BackendChallenge.CrossCutting.Endpoints;
using BackendChallenge.CrossCutting.Storage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BackendChallenge.Application.Delivery.UseCases;
public static class UploadCnhImage
{
    public record HandlerRequest(
        Guid DeliverymanId, string FileName, string ContentType, byte[] ImageBlob) : ICommand<Response>;
    public record Response(Guid Id, string ImageUrl);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/deliverymen/{id}/upload-cnh", async (
                Guid id,
                [FromForm] IFormFile image,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (image == null || (image.ContentType != "image/png" && image.ContentType != "image/bmp"))
                {
                    return Results.BadRequest("Invalid image type. Only PNG and BMP are allowed.");
                }

                using MemoryStream memoryStream = new();
                await image.CopyToAsync(memoryStream, cancellationToken);
                var imageBytes = memoryStream.ToArray();

                HandlerRequest handlerRequest = new(id, image.FileName, image.ContentType, imageBytes);

                var result = await sender.Send(handlerRequest, cancellationToken);

                if (result.IsFailure)
                    return Results.BadRequest(result);

                return Results.Ok(result);

            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Deliveryman })
            .DisableAntiforgery()
            .WithTags("Deliveryman");
        }
    }

    internal sealed class Handler(
       ApplicationDbContext context,
       IStorageService storageService) : ICommandHandler<HandlerRequest, Response>
    {
        public async Task<Result<Response>> Handle(HandlerRequest request, CancellationToken cancellationToken)
        {
            var deliveryman = await context.Deliveryman.FindAsync(request.DeliverymanId);

            if (deliveryman is null)
                return Result.Failure<Response>(DomainErrors.NotFound);

            var blobName = $"{request.DeliverymanId}/{request.FileName}";
            var imageUrl = await storageService.UploadImageAsync(request.ImageBlob, blobName);

            deliveryman.UpdateCnhImageUrl(imageUrl);

            await context.SaveChangesAsync(cancellationToken);

            var response = new Response(request.DeliverymanId, imageUrl);
            return Result.Success(response);
        }
    }
}
