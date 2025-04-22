using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpGameService.Core.Configuration;
using SharpGameService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core.Hosting
{
    public abstract class CoreHost(ILogger<CoreHost> logger, IOptions<SharpGameServiceOptions> options, IHouse house) : BackgroundService
    {
        private readonly SharpGameServiceOptions _options = options.Value;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.House.SinglePlayer)
            {
                house.CreateRoom(_options.Rooms.SinglePlayerRoomId, _options.Rooms.SinglePlayerRoomCode);
                logger.LogInformation("Service set to single player mode, room created with id of singleplayer and no room code.");
            }
            logger.LogInformation("Started core game service.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service is stopping, closing all rooms.");
            await house.CloseHouse();
            logger.LogInformation("All rooms closed.");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await house.ProcessAsync();
                await Task.Delay(1000 / _options.House.TicksPerSecond, stoppingToken);
                await Task.Yield();
            }
        }
    }
}
