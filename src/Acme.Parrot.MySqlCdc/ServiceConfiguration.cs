using Acme.Parrot.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySqlCdc;

namespace Acme.Parrot.MySqlCdc;

public static class ServiceConfiguration
{
    public static IServiceCollection UseMySqlCdcClient(this IServiceCollection services, ReplicaOptions _options)
    {
        // services.ConfigureOptions(options);
        services.Configure<ReplicaOptions>(options =>
        {
            options.Hostname = _options.Hostname;
            options.Port = _options.Port;
            options.Username = _options.Username;
            options.Password = _options.Password;
            options.Database = _options.Database;
            options.SslMode = _options.SslMode;
            options.HeartbeatInterval = _options.HeartbeatInterval;
            options.Blocking = _options.Blocking;
        });
        services.TryAddSingleton<IBinlogEventHandler, DefaultBinlogEventHandler>();
        services.TryAddSingleton<MySqlCdcClientFactory>();
        // services.Replace(new ServiceDescriptor(typeof(ILeaderMajorJob), typeof(MySqlCdcJob)));
        services.Replace(ServiceDescriptor.Singleton<ILeaderMajorJob, MySqlCdcJob>());
        return services;
    }
}