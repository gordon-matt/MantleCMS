using Microsoft.Extensions.FileProviders;

namespace Mantle.Web.Mvc.EmbeddedResources;

public class EmbeddedViewFileProvider : IFileProvider
{
    private static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars()
        .Where(c => c is not '/' and not '\\')
        .ToArray();

    private EmbeddedResourceTable embeddedViews;

    private EmbeddedResourceTable EmbeddedViews
    {
        get
        {
            if (embeddedViews == null)
            {
                var embeddedResourceResolver = DependoResolver.Instance.Resolve<IEmbeddedResourceResolver>();
                embeddedViews = embeddedResourceResolver.Views;
            }
            return embeddedViews;
        }
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (string.IsNullOrEmpty(subpath))
        {
            return new NotFoundFileInfo(subpath);
        }

        if (subpath.StartsWith("/", StringComparison.Ordinal))
        {
            subpath = subpath[1..];
        }

        if (!IsEmbeddedView(subpath))
        {
            return null;
        }

        var metadata = EmbeddedViews.FindEmbeddedResource(subpath);

        return metadata == null ? null : (IFileInfo)new EmbeddedResourceFileInfo(metadata);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        // The file name is assumed to be the remainder of the resource name.
        if (subpath == null)
        {
            return NotFoundDirectoryContents.Singleton;
        }

        // Relative paths starting with a leading slash okay
        if (subpath.StartsWith("/", StringComparison.Ordinal))
        {
            subpath = subpath[1..];
        }

        // Non-hierarchal.
        if (!subpath.Equals(string.Empty))
        {
            return NotFoundDirectoryContents.Singleton;
        }

        var entries = new List<IFileInfo>();

        // TODO: The list of resources in an assembly isn't going to change. Consider caching.
        var resources = EmbeddedViews.Resources;
        foreach (var resource in resources)
        {
            entries.Add(new EmbeddedResourceFileInfo(resource));
        }

        return new EnumerableDirectoryContents(entries);
    }

    public IChangeToken Watch(string pattern) => NullChangeToken.Singleton;

    private static bool HasInvalidPathChars(string path) => path.IndexOfAny(invalidFileNameChars) != -1;

    private bool IsEmbeddedView(string subpath) => !string.IsNullOrEmpty(subpath) && EmbeddedViews.ContainsEmbeddedResource(subpath);
}