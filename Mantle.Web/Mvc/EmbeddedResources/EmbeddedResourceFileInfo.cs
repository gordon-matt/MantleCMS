using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mantle.Infrastructure;
using Microsoft.Extensions.FileProviders;

namespace Mantle.Web.Mvc.EmbeddedResources
{
    public class EmbeddedResourceFileInfo : IFileInfo
    {
        private readonly EmbeddedResourceMetadata metadata;
        private readonly Assembly assembly;
        private long? length;

        public EmbeddedResourceFileInfo(EmbeddedResourceMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            this.metadata = metadata;
            assembly = GetResourceAssembly();
            Name = metadata.ResourceName;
            LastModified = DateTimeOffset.UtcNow;
        }

        public bool Exists => true;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified { get; }

        public long Length
        {
            get
            {
                if (!length.HasValue)
                {
                    using (var stream = assembly.GetManifestResourceStream(metadata.ResourceName))
                    {
                        length = stream.Length;
                    }
                }
                return length.Value;
            }
        }

        public string Name { get; }

        public string PhysicalPath => null;

        public Stream CreateReadStream()
        {
            var stream = assembly.GetManifestResourceStream(metadata.ResourceName);
            if (!length.HasValue)
            {
                length = stream.Length;
            }
            return stream;
        }

        protected virtual Assembly GetResourceAssembly()
        {
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();

            return typeFinder.GetAssemblies().FirstOrDefault(assembly =>
                string.Equals(assembly.FullName, metadata.AssemblyFullName, StringComparison.OrdinalIgnoreCase));
        }
    }
}