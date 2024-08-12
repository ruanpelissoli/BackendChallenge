using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Bikes;
internal class DomainErrors
{
    public static Error NotFound = new(
       "Bike.NotFound",
       "Bike not found");
}
