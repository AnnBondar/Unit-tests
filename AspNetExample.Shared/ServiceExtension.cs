using AspNetExample.Service;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetExample.Shared;

public static class ServiceExtension
{
    public static void RegisterService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IGuidProvider, GuidProvider>();
        serviceCollection.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}