using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Rentals;
public class Plan : Entity<Guid>
{
    public Plan(int durationInDays, decimal costPerDay)
    {
        DurationInDays = durationInDays;
        CostPerDay = costPerDay;
    }

    public Plan(int durationInDays, decimal costPerDay, decimal fineCostPercentagePerDay)
    {
        DurationInDays = durationInDays;
        CostPerDay = costPerDay;
        FineCostPercentagePerDay = fineCostPercentagePerDay;
    }

    protected Plan() { }

    public int DurationInDays { get; private set; }
    public decimal CostPerDay { get; private set; }
    public decimal FineCostPercentagePerDay { get; private set; } = 0M;

    public decimal TotalValue => DurationInDays * CostPerDay;
}
