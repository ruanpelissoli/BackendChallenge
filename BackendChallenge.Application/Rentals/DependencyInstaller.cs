using BackendChallenge.CrossCutting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendChallenge.Application.Rentals;
public class DependencyInstaller : IDependencyInjectionInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRepository, Repository>();
    }
}
