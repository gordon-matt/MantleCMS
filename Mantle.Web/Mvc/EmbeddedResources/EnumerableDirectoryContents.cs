using System.Collections;
using Microsoft.Extensions.FileProviders;

namespace Mantle.Web.Mvc.EmbeddedResources;

internal class EnumerableDirectoryContents : IDirectoryContents
{
    private readonly IEnumerable<IFileInfo> entries;

    public EnumerableDirectoryContents(IEnumerable<IFileInfo> entries)
    {
        this.entries = entries ?? throw new ArgumentNullException(nameof(entries));
    }

    public bool Exists => true;

    public IEnumerator<IFileInfo> GetEnumerator() => entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => entries.GetEnumerator();
}