using Microsoft.Extensions.DependencyInjection;

namespace Acme.Parrot.Server;

public static class ServerConfiguration
{
    public static IServiceCollection UseRaftClusterServer(this IServiceCollection services)
    {
        
        services.AddHostedService<ParrotRaftCluster>();
        return services;
    }
}