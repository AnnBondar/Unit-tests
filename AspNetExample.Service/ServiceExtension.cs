using AspNetExample.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetExample.Service;

public static class ServiceExtension
{
    public static void RegisterService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IService, Services.Service>();
        serviceCollection.AddTransient<IModelDescriptionProvider, ModelDescriptionProvider>();
    }
}