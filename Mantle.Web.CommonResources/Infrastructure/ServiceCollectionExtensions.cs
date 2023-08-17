﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.CommonResources.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void ConfigureMantleCommonResourceOptions(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.Configure<MantleCommonResourceOptions>(configuration.GetSection("MantleCommonResourceOptions"));
    }
}