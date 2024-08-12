using BackendChallenge.CrossCutting.Common;

namespace BackendChallenge.Application.Accounts;
internal class DomainErrors
{
    public static Error LoginError = new(
        "Account.LoginError",
        "Invalid username or password");
}
