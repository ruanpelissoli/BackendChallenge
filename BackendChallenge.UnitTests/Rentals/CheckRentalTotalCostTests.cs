using BackendChallenge.Application.Rentals;
using BackendChallenge.Application.Rentals.UseCases;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BackendChallenge.UnitTests.Rentals;
public class CheckRentalTotalCostTests
{
    [Fact]
    public async Task Handler_ReturnsNotFound_WhenRentalIsNull()
    {
        // Arrange
        var repositoryMock = new Mock<IRepository>();
        repositoryMock.Setup(r => r.GetRentalById(It.IsAny<Guid>()))
            .ReturnsAsync((Rental?)null);

        var request = new CheckRentalTotalCost.Request(DateTime.UtcNow);

        // Act
        var result = await CheckRentalTotalCost.Handler(Guid.NewGuid(), request, repositoryMock.Object);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Handler_ReturnsCorrectResponse_WhenReturnedBeforeEndDate()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        var plan = new Plan(20, 100M, 20M);
        var rental = Rental.Create(Guid.NewGuid(), Guid.NewGuid(), plan.Id, plan.TotalValue, startDate, endDate);
        rental.SetPlan(plan);

        var repositoryMock = new Mock<IRepository>();
        repositoryMock.Setup(r => r.GetRentalById(It.IsAny<Guid>()))
            .ReturnsAsync(rental);

        var request = new CheckRentalTotalCost.Request(returnDate.ToDateTime(TimeOnly.MinValue));

        // Act
        var result = await CheckRentalTotalCost.Handler(rentalId, request, repositoryMock.Object);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult.Value as CheckRentalTotalCost.Response;
        response.Should().NotBeNull();
        response.DaysUsedCost.Should().Be(500M); // 5 days * 100M
        response.FineCostPerDaysRemaining.Should().Be(1000M); // 5 days * 100M * 0.2
        response.CostPerAdditionalDays.Should().Be(0M);
        response.TotalCost.Should().Be(1500M);
    }

    [Fact]
    public async Task Handler_ReturnsCorrectResponse_WhenReturnedAfterEndDate()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var plan = new Plan(20, 100M, 20M);
        var rental = Rental.Create(Guid.NewGuid(), Guid.NewGuid(), plan.Id, plan.TotalValue, startDate, endDate);
        rental.SetPlan(plan);

        var repositoryMock = new Mock<IRepository>();
        repositoryMock.Setup(r => r.GetRentalById(It.IsAny<Guid>()))
            .ReturnsAsync(rental);

        var request = new CheckRentalTotalCost.Request(returnDate.ToDateTime(TimeOnly.MinValue));

        // Act
        var result = await CheckRentalTotalCost.Handler(rentalId, request, repositoryMock.Object);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult.Value as CheckRentalTotalCost.Response;
        response.Should().NotBeNull();
        response.DaysUsedCost.Should().Be(2000M); // 20 days * 100M
        response.FineCostPerDaysRemaining.Should().Be(0M);
        response.CostPerAdditionalDays.Should().Be(500M); // 10 days * 50M
        response.TotalCost.Should().Be(2500M);
    }
}