using Microsoft.Extensions.DependencyInjection.Extensions;
using Pilgrimage;

namespace Microsoft.Extensions.DependencyInjection;

public static class PilgrimageServiceCollectionExtensions
{
    public static IServiceCollection AddPilgrimage(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<ISessionFactory, SessionFactory>();

        return services;
    }
}
