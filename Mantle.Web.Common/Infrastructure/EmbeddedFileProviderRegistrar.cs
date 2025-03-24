using Mantle.Web.Infrastructure;
using Microsoft.Extensions.FileProviders;

namespace Mantle.Web.Common.Infrastructure;

public class EmbeddedFileProviderRegistrar : IEmbeddedFileProviderRegistrar
{
    public IEnumerable<EmbeddedFileProvider> EmbeddedFileProviders =>
    [
        new EmbeddedFileProvider(GetType().Assembly, "Mantle.Web.Common")
    ];
}