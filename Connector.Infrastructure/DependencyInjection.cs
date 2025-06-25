using Connector.Infrastructure.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace Connector.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<RestConnector>();
        services.AddScoped<TimePeriodResolver>();
    }
}