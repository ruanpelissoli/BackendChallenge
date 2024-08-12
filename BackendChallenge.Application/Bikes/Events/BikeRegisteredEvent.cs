using MassTransit;
using Microsoft.Extensions.Logging;

namespace BackendChallenge.Application.Bikes.Events;
public record BikeRegisteredEvent(int Year, string Model, string LicensePlate);

public sealed class BikeRegisteredConsumer(
    IPublishEndpoint publisher,
    ApplicationDbContext dbContext,
    ILogger<BikeRegisteredConsumer> logger) : IConsumer<BikeRegisteredEvent>

{
    public async Task Consume(ConsumeContext<BikeRegisteredEvent> context)
    {
        logger.LogInformation("Starting new bike registration: {Year} {Model} {LicensePlate}", context.Message.Year, context.Message.Model, context.Message.LicensePlate);

        var bike = Bike.Create(context.Message.Year, context.Message.Model, context.Message.LicensePlate);

        await dbContext.Bikes.AddAsync(bike);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        if (bike.Year == DateTime.Now.Year)
            await publisher.Publish(new BikeFromThisYearRegisteredEvent(bike));

        logger.LogInformation("Bike registered: {Bike}", bike);
    }
}