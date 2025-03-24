using Mantle.Web.Infrastructure;
using Microsoft.Extensions.FileProviders;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure;

public class EmbeddedFileProviderRegistrar : IEmbeddedFileProviderRegistrar
{
    public IEnumerable<EmbeddedFileProvider> EmbeddedFileProviders =>
    [
        new EmbeddedFileProvider(GetType().Assembly, "Mantle.Plugins.Messaging.Forums")
    ];
}