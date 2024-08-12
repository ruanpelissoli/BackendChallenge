using BackendChallenge.Application.Bikes;
using BackendChallenge.Application.Delivery;
using BackendChallenge.Application.Rentals;
using BackendChallenge.Application.Rentals.UseCases;
using FluentAssertions;
using Moq;

namespace BackendChallenge.UnitTests.Rentals;
public class RentABikeTests
{
    private readonly Mock<IRepository> _repositoryMock;
    private RentABike.Handler? _sut;

    public RentABikeTests()
    {
        _repositoryMock = new Mock<IRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBikeNotAvailableForRental()
    {
        // Arrange
        var request = new RentABike.HandlerRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "1234qqwe"
        );

        var deliveryman = Deliveryman.Create(request.AccountId, It.IsAny<string>(), It.IsAny<string>(), DateTime.Now.AddYears(-20), "123445", CnhType.A);
        var bike = Bike.Create(2024, "TestModel", "ABC1234");
        var plan = new Plan(7, 100M, 20M);

        var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var endDate = startDate.AddDays(plan.DurationInDays);

        _repositoryMock.Setup(r => r.GetDelivymanByAccountId(request.AccountId))
                       .ReturnsAsync(deliveryman);

        _repositoryMock.Setup(r => r.GetBikeById(request.BikeId))
                       .ReturnsAsync(bike);

        _repositoryMock.Setup(r => r.GetPlanById(request.PlanId))
                       .ReturnsAsync(plan);

        _repositoryMock.Setup(r => r.IsBikeAvailableForRental(request.BikeId, startDate, endDate))
                       .ReturnsAsync(false);

        // Act
        _sut = new RentABike.Handler(_repositoryMock.Object);
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.First().Message.Should().Be(Application.Rentals.DomainErrors.BikeNotAvailable.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenBikeIsAvailableForRental()
    {
        // Arrange
        var bike = Bike.Create(2024, "TestModel", "ABC1234");
        var plan = new Plan(7, 100M, 20M);

        var request = new RentABike.HandlerRequest(
           bike.Id,
           plan.Id,
           "1234qqwe"
        );

        var deliveryman = Deliveryman.Create(request.AccountId, It.IsAny<string>(), It.IsAny<string>(), DateTime.Now.AddYears(-20), "123445", CnhType.A);


        var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var endDate = startDate.AddDays(plan.DurationInDays);

        _repositoryMock.Setup(r => r.GetDelivymanByAccountId(request.AccountId))
                       .ReturnsAsync(deliveryman);

        _repositoryMock.Setup(r => r.GetBikeById(request.BikeId))
                       .ReturnsAsync(bike);

        _repositoryMock.Setup(r => r.GetPlanById(request.PlanId))
                       .ReturnsAsync(plan);

        _repositoryMock.Setup(r => r.IsBikeAvailableForRental(request.BikeId, startDate, endDate))
                       .ReturnsAsync(true);

        _repositoryMock.Setup(r => r.CreateRental(It.IsAny<Rental>()))
                       .Verifiable();

        _repositoryMock.Setup(r => r.Commit())
                       .Returns(Task.CompletedTask)
                       .Verifiable();

        // Act
        _sut = new RentABike.Handler(_repositoryMock.Object);
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Bike.BikeId.Should().Be(request.BikeId);

        _repositoryMock.Verify(r => r.CreateRental(It.IsAny<Rental>()), Times.Once);
        _repositoryMock.Verify(r => r.Commit(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCnhTypeIsInvalid()
    {
        // Arrange
        var request = new RentABike.HandlerRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "1234qqwe"
        );

        var deliveryman = Deliveryman.Create(request.AccountId, It.IsAny<string>(), It.IsAny<string>(), DateTime.Now.AddYears(-20), "123445", CnhType.B);

        _repositoryMock.Setup(r => r.GetDelivymanByAccountId(request.AccountId))
                       .ReturnsAsync(deliveryman);

        // Act
        _sut = new RentABike.Handler(_repositoryMock.Object);
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.First().Message.Should().Be(Application.Rentals.DomainErrors.InvalidCnhType.Message);
    }

}
