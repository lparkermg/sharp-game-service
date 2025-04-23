using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpGameService.Core.Configuration;
using SharpGameService.Core.Interfaces;

namespace SharpGameService.Core.Hosting
{
    /// <summary>
    /// The default implementation of the core host.
    /// </summary>
    /// <param name="logger">The <see cref="CoreHost"/> logger.</param>
    /// <param name="options">Configuration for the host.</param>
    /// <param name="house">The services house.</param>
    public sealed class DefaultCoreHost(ILogger<CoreHost> logger, IOptions<SharpGameServiceOptions> options, IHouse house) : CoreHost(logger, options, house)
    {
    }
}
