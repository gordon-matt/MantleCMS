﻿using System.Collections.Generic;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.FileProviders;

namespace Mantle.Web.CommonResources.Infrastructure
{
    public class EmbeddedFileProviderRegistrar : IEmbeddedFileProviderRegistrar
    {
        public IEnumerable<EmbeddedFileProvider> EmbeddedFileProviders => new List<EmbeddedFileProvider>
        {
            new EmbeddedFileProvider(GetType().Assembly, "Mantle.Web.CommonResources")
        };
    }
}