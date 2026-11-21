using Dependo;
using Mantle.Web.Configuration;
using Mantle.Web.Infrastructure;
using Mantle.Web.Mvc.Routing;

[assembly: DependoExports(typeof(ISettings), typeof(IRouteProvider), typeof(IEmbeddedFileProviderRegistrar))]
