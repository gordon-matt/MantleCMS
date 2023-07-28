using Microsoft.Extensions.FileProviders;

namespace Mantle.Web.Infrastructure;

public interface IEmbeddedFileProviderRegistrar
{
    IEnumerable<EmbeddedFileProvider> EmbeddedFileProviders { get; }
}

public class EmbeddedFileProviderRegistrar : IEmbeddedFileProviderRegistrar
{
    public IEnumerable<EmbeddedFileProvider> EmbeddedFileProviders => new List<EmbeddedFileProvider>
    {
        new EmbeddedFileProvider(GetType().Assembly, "Mantle.Web")
    };
}