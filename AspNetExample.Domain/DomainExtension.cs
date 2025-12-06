using AspNetExample.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetExample.Domain;

public static class DomainExtension
{
    public static void RegisterDomain(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRepository, Repository>();
        serviceCollection.AddSingleton<IStorage, Storage>();
    }
}