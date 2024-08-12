namespace BackendChallenge.Application.Rentals.Services;

public record BikeReturnCalculationResponse(
    decimal DaysUsedCost,
    decimal FineCostPerDaysRemaining,
    decimal CostPerAdditionalDays,
    decimal TotalCost);

public interface IBikeRentalReturnCalculation
{
    BikeReturnCalculationResponse CalculateTotalCost(DateOnly returnDate, Rental rental);
}

public class BikeRentalReturnCalculation : IBikeRentalReturnCalculation
{
    private static decimal CalculateDaysUsedCost(int daysUsed, decimal costPerDay)
    {
        return daysUsed * costPerDay;
    }

    private static decimal CalculateFine(int remainingDays, decimal costPerDay, decimal finePercentage)
    {
        return (remainingDays * costPerDay) * finePercentage;
    }

    private static decimal CalculateAdditionalDaysCost(int additionalDays, decimal additionalCostPerDay)
    {
        return additionalDays * additionalCostPerDay;
    }

    public BikeReturnCalculationResponse CalculateTotalCost(DateOnly returnDate, Rental rental)
    {
        if (returnDate < rental.EndDate)
        {
            var remainingDaysToFine = rental.EndDate.DayNumber - returnDate.DayNumber;
            var daysUsedToCharge = returnDate.DayNumber - rental.StartDate.DayNumber;

            var daysUsedCost = CalculateDaysUsedCost(daysUsedToCharge, rental.Plan.CostPerDay);

            var finePercentage = rental.Plan.FineCostPercentagePerDay;
            var fineCostPerDaysRemaining = CalculateFine(remainingDaysToFine, rental.Plan.CostPerDay, finePercentage);

            var costPerAdditionalDays = 0M;

            var totalCost = daysUsedCost + fineCostPerDaysRemaining + costPerAdditionalDays;

            return new BikeReturnCalculationResponse(daysUsedCost, fineCostPerDaysRemaining, costPerAdditionalDays, totalCost);
        }
        else if (returnDate == rental.EndDate)
        {
            var daysUsedToCharge = rental.Plan.DurationInDays;

            var daysUsedCost = CalculateDaysUsedCost(daysUsedToCharge, rental.Plan.CostPerDay);
            var fineCostPerDaysRemaining = 0M;
            var costPerAdditionalDays = 0M;

            var totalCost = daysUsedCost + fineCostPerDaysRemaining + costPerAdditionalDays;

            return new BikeReturnCalculationResponse(daysUsedCost, fineCostPerDaysRemaining, costPerAdditionalDays, totalCost);
        }
        else
        {
            var daysUsedToCharge = rental.Plan.DurationInDays;
            var additionalDays = returnDate.DayNumber - rental.EndDate.DayNumber;

            var daysUsedCost = CalculateDaysUsedCost(daysUsedToCharge, rental.Plan.CostPerDay);

            var fineCostPerDaysRemaining = 0M;

            var costPerAdditionalDays = CalculateAdditionalDaysCost(additionalDays, 50M);

            var totalCost = daysUsedCost + fineCostPerDaysRemaining + costPerAdditionalDays;

            return new BikeReturnCalculationResponse(daysUsedCost, fineCostPerDaysRemaining, costPerAdditionalDays, totalCost);
        }
    }
}
