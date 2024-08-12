using MassTransit;
using Microsoft.Extensions.Logging;

namespace BackendChallenge.Application.Bikes.Events;
public record BikeFromThisYearRegisteredEvent(Bike Bike);

public sealed class BikeFromThisYearRegisteredConsumer(
    ILogger<BikeFromThisYearRegisteredConsumer> logger) : IConsumer<BikeFromThisYearRegisteredEvent>

{
    public async Task Consume(ConsumeContext<BikeFromThisYearRegisteredEvent> context)
    {
        logger.LogInformation("Bike from this year registered: {Bike}", context.Message.Bike);

        await Task.CompletedTask;
    }
}
