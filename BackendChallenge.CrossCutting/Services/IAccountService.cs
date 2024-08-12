namespace BackendChallenge.CrossCutting.Services;
public interface IAccountService<T>
{
    Task<T?> CreateAccount(T account, string password, string role);
    Task<T?> FindByNameAsync(string username);
    Task<bool> CheckPasswordAsync(T account, string password);
    Task<IList<string>> GetRolesAsync(T account);
    Task RemoveFromRoleAsync(T account, string role);
    Task DeleteAsync(T account);
}
