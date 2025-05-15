using Dependo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Mantle.Infrastructure;

public interface IMantleStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="containerBuilder">Container Builder</param>
    /// <param name="configuration">Configuration root of the application</param>
    void ConfigureServices(IContainerBuilder containerBuilder, IConfiguration configuration);

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    void Configure(IApplicationBuilder application);

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    int Order { get; }
}