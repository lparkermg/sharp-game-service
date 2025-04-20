using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SharpGameService.Core.Configuration;
using SharpGameService.Core;
using SharpGameService.Core.Interfaces;

namespace SharpGameService.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSharpGameService<TImplementationType, TRoomType>(this IServiceCollection services, Action<SharpGameServiceOptions> options) where TImplementationType : BackgroundService, IHostedService where TRoomType : BaseRoom, new()
        {
            services.Configure<SharpGameServiceOptions>(options);

            services.AddHostedService<TImplementationType>();
            services.AddSingleton<IHouse, House<TRoomType>>();
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
