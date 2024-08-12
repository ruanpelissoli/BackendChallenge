using BackendChallenge.CrossCutting.Services;
using Microsoft.AspNetCore.Identity;

namespace BackendChallenge.Application.Accounts;
internal class AccountService(
    UserManager<Account> _userManager) : IAccountService<Account>
{
    public async Task<bool> CheckPasswordAsync(Account account, string password)
    {
        return await _userManager.CheckPasswordAsync(account, password);
    }

    public async Task<Account?> CreateAccount(Account account, string password, string role)
    {
        var result = await _userManager.CreateAsync(account, password);

        if (!result.Succeeded)
            return null;

        await _userManager.AddToRoleAsync(account, Roles.Deliveryman);

        return account;
    }

    public async Task DeleteAsync(Account account)
    {
        await _userManager.DeleteAsync(account);
    }

    public async Task<Account?> FindByNameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<IList<string>> GetRolesAsync(Account account)
    {
        return await _userManager.GetRolesAsync(account);
    }

    public async Task RemoveFromRoleAsync(Account account, string role)
    {
        await _userManager.RemoveFromRoleAsync(account, role);
    }
}
