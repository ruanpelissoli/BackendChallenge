using MassTransit;

namespace BackendChallenge.Application.Bikes.Events;
public record BikeRegisteredEvent(int Year, string Model, string LicensePlate);

public sealed class BikeRegisteredConsumer(
    ApplicationDbContext _context) : IConsumer<BikeRegisteredEvent>

{
    public async Task Consume(ConsumeContext<BikeRegisteredEvent> context)
    {
        var bike = Bike.Create(context.Message.Year, context.Message.Model, context.Message.LicensePlate);

        await _context.Bikes.AddAsync(bike);
        await _context.SaveChangesAsync(context.CancellationToken);
    }
}