namespace HubPedidos.Application;

using System.Threading.Channels;
using HubPedidos.Application.Data;
using HubPedidos.Application.Jobs;
using HubPedidos.Application.Services;
using HubPedidos.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services.AddSingleton<CatalogoProcessadores>();
        services.AddScoped<PedidoConsultaService>();
        services.AddSingleton<QueueMetricsService>();

        var channel = Channel.CreateBounded<ProcessarPedidoJob>(new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });

        services.AddSingleton(channel);
        services.AddSingleton(channel.Reader);
        services.AddSingleton(channel.Writer);
        services.AddHostedService<PedidoQueueWorker>();

        return services;
    }

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<HubPedidosDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
