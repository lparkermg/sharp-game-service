using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SharpGameService.Core.Configuration;

namespace SharpGameService.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSharpGameService<TImplementationType>(this IServiceCollection services, Action<SharpGameServiceOptions> options) where TImplementationType : BackgroundService, IHostedService
        {
            services.Configure<SharpGameServiceOptions>(options);

            services.AddHostedService<TImplementationType>();
            return services;
        }

        public static IApplicationBuilder UseSharpGameService(this IApplicationBuilder host, WebSocketOptions socketOptions)
        {
            // Setup any settings for the service.

            // We should be using controllers, but they would likely be mapped in the application rather than the package.

            // Setup websockets
            host.UseWebSockets(socketOptions);
            return host;
        }
    }
}
