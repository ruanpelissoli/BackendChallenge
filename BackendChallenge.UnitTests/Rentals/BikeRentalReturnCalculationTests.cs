using BackendChallenge.Application.Rentals;
using BackendChallenge.Application.Rentals.Services;
using FluentAssertions;

namespace BackendChallenge.UnitTests.Rentals;
public class BikeRentalReturnCalculationTests
{
    private IBikeRentalReturnCalculation? _sut;

    [Fact]
    public void WhenReturnedBeforeEndDate()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        var plan = new Plan(20, 100M, 0.2M);
        var rental = Rental.Create(Guid.NewGuid(), Guid.NewGuid(), plan.Id, plan.TotalValue, startDate, endDate);
        rental.SetPlan(plan);

        // Act
        _sut = new BikeRentalReturnCalculation();
        var result = _sut.CalculateTotalCost(returnDate, rental);

        // Assert
        result.Should().NotBeNull();
        result.DaysUsedCost.Should().Be(1500M); // 15 days * 100M
        result.FineCostPerDaysRemaining.Should().Be(100M); // 5 days * 100M * 0.2
        result.CostPerAdditionalDays.Should().Be(0M);
        result.TotalCost.Should().Be(1600M);
    }

    [Fact]
    public void WhenReturnedAfterEndDate()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var plan = new Plan(20, 100M, 20M);
        var rental = Rental.Create(Guid.NewGuid(), Guid.NewGuid(), plan.Id, plan.TotalValue, startDate, endDate);
        rental.SetPlan(plan);

        // Act
        _sut = new BikeRentalReturnCalculation();
        var result = _sut.CalculateTotalCost(returnDate, rental);

        // Assert
        result.Should().NotBeNull();
        result.DaysUsedCost.Should().Be(2000M); // 20 days * 100M
        result.FineCostPerDaysRemaining.Should().Be(0M);
        result.CostPerAdditionalDays.Should().Be(500M); // 10 days * 50M
        result.TotalCost.Should().Be(2500M);
    }

    [Fact]
    public void WhenReturnedAtTheCorrectDate()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var plan = new Plan(20, 100M, 20M);
        var rental = Rental.Create(Guid.NewGuid(), Guid.NewGuid(), plan.Id, plan.TotalValue, startDate, endDate);
        rental.SetPlan(plan);

        // Act
        _sut = new BikeRentalReturnCalculation();
        var result = _sut.CalculateTotalCost(returnDate, rental);

        // Assert
        result.Should().NotBeNull();
        result.DaysUsedCost.Should().Be(2000M); // 20 days * 100M
        result.FineCostPerDaysRemaining.Should().Be(0M);
        result.CostPerAdditionalDays.Should().Be(0M);
        result.TotalCost.Should().Be(2000M);
    }
}