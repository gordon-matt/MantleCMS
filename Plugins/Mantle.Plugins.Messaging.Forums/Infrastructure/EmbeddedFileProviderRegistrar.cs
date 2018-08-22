using System.Collections.Generic;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.FileProviders;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure
{
    public class EmbeddedFileProviderRegistrar : IEmbeddedFileProviderRegistrar
    {
        public IEnumerable<EmbeddedFileProvider> EmbeddedFileProviders => new List<EmbeddedFileProvider>
        {
            new EmbeddedFileProvider(GetType().Assembly, "Mantle.Plugins.Messaging.Forums")
        };
    }
}