using Dependo;
using Mantle.Data.Entity;
using Mantle.Web.Configuration;
using Mantle.Web.Infrastructure;

[assembly: DependoExports(
    typeof(ISettings),
    typeof(IEmbeddedFileProviderRegistrar),
    typeof(IMantleEntityTypeConfiguration))]
