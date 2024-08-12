using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Delivery;
internal class DomainErrors
{
    public static Error NotFound = new(
       "Deliveryman.NotFound",
       "Deliveryman not found");

    public static Error FailedToCreateAccount = new(
       "Deliveryman.FailedToCreateAccount",
       "Failed to create deliveryman account");

    public static Error FailedToCreate = new(
       "Deliveryman.FailedToCreate",
       "An error occurred while registering the deliveryman.");
}
