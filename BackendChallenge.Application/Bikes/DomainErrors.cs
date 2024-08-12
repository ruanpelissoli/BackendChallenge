using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Bikes;
internal class DomainErrors
{
    public static Error NotFound = new(
       "Bike.NotFound",
       "Bike not found");

    public static Error BikeHasRentalRecord = new(
       "Bike.BikeHasRentalRecord",
       "Bike has rental record and cannot be deleted.");
}
