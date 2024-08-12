using BackendChallenge.Application.Bikes;
using BackendChallenge.Application.Delivery;
using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Rentals;

public class Rental : Entity<Guid>
{
    private Rental(
        Guid bikeId, Guid deliverymanId, Guid planId, decimal totalCost, DateOnly startDate, DateOnly endDate)
    {
        BikeId = bikeId;
        DeliverymanId = deliverymanId;
        PlanId = planId;
        TotalCost = totalCost;
        StartDate = startDate;
        EndDate = endDate;
    }

    protected Rental() { }

    public Guid BikeId { get; private set; }
    public Guid DeliverymanId { get; private set; }
    public Guid PlanId { get; private set; }
    public decimal TotalCost { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public Bike Bike { get; private set; }
    public Deliveryman Deliveryman { get; private set; }
    public Plan Plan { get; private set; }

    public static Rental Create(
        Guid bikeId,
        Guid deliverymanId,
        Guid planId,
        decimal totalCost,
        DateOnly startDate,
        DateOnly endDate) =>
        new(bikeId, deliverymanId, planId, totalCost, startDate, endDate);

    public void SetPlan(Plan plan) => Plan = plan;

}
