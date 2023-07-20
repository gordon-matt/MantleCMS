using Mantle.Web.Infrastructure;
using Microsoft.Extensions.FileProviders;

namespace Mantle.Web.Common.Infrastructure
{
    public class EmbeddedFileProviderRegistrar : IEmbeddedFileProviderRegistrar
    {
        public IEnumerable<EmbeddedFileProvider> EmbeddedFileProviders => new List<EmbeddedFileProvider>
        {
            new EmbeddedFileProvider(GetType().Assembly, "Mantle.Web.Common")
        };
    }
}