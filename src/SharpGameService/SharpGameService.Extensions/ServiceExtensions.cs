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
        /// <summary>
        /// Adds the SharpGameService to the service collection.
        /// </summary>
        /// <typeparam name="TServiceImplementationType">The implementation type of the hosted service.</typeparam>
        /// <typeparam name="TRoomType">The custom room type (this is where the game logic will be processed and handled)</typeparam>
        /// <param name="services">The current service collection.</param>
        /// <param name="options">The configuration setup.</param>
        /// <returns>An updated service collection.</returns>
        public static IServiceCollection AddSharpGameService<TServiceImplementationType, TRoomType>(this IServiceCollection services, Action<SharpGameServiceOptions> options) 
            where TServiceImplementationType : BackgroundService, IHostedService 
            where TRoomType : BaseRoom, new()
        {
            services.Configure<SharpGameServiceOptions>(options);

            services.AddHostedService<TServiceImplementationType>();
            services.AddSingleton<IHouse, House<TRoomType>>();

            return services;
        }

        /// <summary>
        /// Uses SharpGameService in the application.
        /// 
        /// This does not setup the controller mappings, app.MapControllers() should be used in the application to map them.
        /// </summary>
        /// <param name="host">The application builder host.</param>
        /// <param name="socketOptions">Settings for the web sockets.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseSharpGameService(this IApplicationBuilder host, WebSocketOptions socketOptions)
        {
            // We should be using controllers, but they will need to be mapped in the application rather than the package.

            // Setup websockets
            host.UseWebSockets(socketOptions);
            return host;
        }
    }
}
