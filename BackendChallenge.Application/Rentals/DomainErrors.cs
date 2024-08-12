using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Rentals;
internal class DomainErrors
{
    public static Error NotFound = new(
        "Rental.NotFound",
        "The rental not found");

    public static Error InvalidCnhType = new(
        "Rental.InvalidCnhType",
        "Only deliveryman with CNH type A can rent a bike.");

    public static Error BikeNotAvailable = new(
        "Rental.BikeNotAvailableForThePeriod",
        "Bike is not available for the period.");
}
