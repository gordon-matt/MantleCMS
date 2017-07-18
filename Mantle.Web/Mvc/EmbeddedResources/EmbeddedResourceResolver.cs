using System.Reflection;
using Mantle.Collections;
using Mantle.Infrastructure;

namespace Mantle.Web.Mvc.EmbeddedResources
{
    public class EmbeddedResourceResolver : IEmbeddedResourceResolver
    {
        private ITypeFinder typeFinder;

        private static EmbeddedResourceTable scripts;
        private static EmbeddedResourceTable styles;
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

        public EmbeddedResourceTable Styles
        {
            get
            {
                if (styles == null)
                {
                    GetEmbeddedResources();
                }
                return styles;
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
            styles = new EmbeddedResourceTable();
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

                    if (key.Contains(".scripts."))
                    {
                        scripts.AddResource(name, assembly.FullName);
                    }
                    else if (key.Contains(".content."))
                    {
                        styles.AddResource(name, assembly.FullName);
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
}