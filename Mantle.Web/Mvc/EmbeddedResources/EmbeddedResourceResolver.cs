namespace Mantle.Web.Mvc.EmbeddedResources;

public class EmbeddedResourceResolver : IEmbeddedResourceResolver
{
    private ITypeFinder typeFinder;

    private static EmbeddedResourceTable scripts;
    private static EmbeddedResourceTable content;
    private static EmbeddedResourceTable views;

    public EmbeddedResourceResolver(ITypeFinder typeFinder)
    {
        this.typeFinder = typeFinder;
    }

    #region IEmbeddedResourceResolver Members

    public EmbeddedResourceTable Scripts
    {
        get
        {
            if (scripts == null)
            {
                GetEmbeddedResources();
            }
            return scripts;
        }
    }

    public EmbeddedResourceTable Content
    {
        get
        {
            if (content == null)
            {
                GetEmbeddedResources();
            }
            return content;
        }
    }

    public EmbeddedResourceTable Views
    {
        get
        {
            if (views == null)
            {
                GetEmbeddedResources();
            }
            return views;
        }
    }

    #endregion IEmbeddedResourceResolver Members

    private void GetEmbeddedResources()
    {
        scripts = new EmbeddedResourceTable();
        content = new EmbeddedResourceTable();
        views = new EmbeddedResourceTable();

        var assemblies = typeFinder.GetAssemblies();

        if (assemblies.IsNullOrEmpty())
        {
            return;
        }

        foreach (var assembly in assemblies)
        {
            var names = GetNamesOfAssemblyResources(assembly);

            if (names.IsNullOrEmpty())
            {
                continue;
            }

            foreach (var name in names)
            {
                var key = name.ToLowerInvariant();

                if (key.ContainsAny(".wwwroot.js.", ".scripts."))
                {
                    scripts.AddResource(name, assembly.FullName);
                }
                else if (key.ContainsAny(".wwwroot.css.", ".content."))
                {
                    content.AddResource(name, assembly.FullName);
                }
                else if (key.Contains(".views."))
                {
                    views.AddResource(name, assembly.FullName);
                }
            }
        }
    }

    private static string[] GetNamesOfAssemblyResources(Assembly assembly)
    {
        //GetManifestResourceNames will throw a NotSupportedException when run on a dynamic assembly
        try
        {
            if (!assembly.IsDynamic)
            {
                return assembly.GetManifestResourceNames();
            }
        }
        catch
        {
            // Any exception we fall back to returning an empty array.
        }

        return new string[] { };
    }
}