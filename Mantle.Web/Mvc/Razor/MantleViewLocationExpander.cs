using System.Collections.Generic;
using System.Linq;
using Mantle.Infrastructure;
using Mantle.Web.Mvc.Controllers;
using Mantle.Web.Mvc.Themes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Mantle.Web.Mvc.Razor
{
    public class MantleViewLocationExpander : IViewLocationExpander
    {
        protected static IEnumerable<ILocationFormatProvider> locationFormatProviders;
        protected static Dictionary<string, List<string>> allViewLocationFormats;
        protected static Dictionary<string, List<string>> allAreaViewLocationFormats;

        public MantleViewLocationExpander()
        {
            if (locationFormatProviders == null)
            {
                locationFormatProviders = EngineContext.Current.ResolveAll<ILocationFormatProvider>() ?? Enumerable.Empty<ILocationFormatProvider>();
                allViewLocationFormats = locationFormatProviders.ToDictionary(k => k.GetType().Assembly.FullName, v => GetViewLocationFormats(v));
                allAreaViewLocationFormats = locationFormatProviders.ToDictionary(k => k.GetType().Assembly.FullName, v => GetAreaViewLocationFormats(v));
            }
        }

        public virtual void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.ViewName.Contains('.'))
            {
                viewLocations = new[]
                {
                    "/" + context.ViewName + ".cshtml" // Embedded Views
                }
                .Concat(viewLocations);
            }

            string controllerAssemblyName = null;

            var controllerActionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
                controllerAssemblyName = controllerTypeInfo.Assembly.FullName;
            }
            else
            {
                var controllerContext = new ControllerContext(context.ActionContext);
                var factory = CreateControllerFactory();
                var controller = factory.CreateController(controllerContext);
                controllerAssemblyName = controller.GetType().Assembly.FullName;
            }

            // Remove everything except base (main app) paths and the plugin paths

            // TODO: Test
            if (!string.IsNullOrEmpty(controllerAssemblyName))
            {
                var validViewLocationFormats = allViewLocationFormats
                    .Where(x => x.Key == controllerAssemblyName)
                    .SelectMany(x => x.Value);

                var validAreaViewLocationFormats = allAreaViewLocationFormats
                    .Where(x => x.Key == controllerAssemblyName)
                    .SelectMany(x => x.Value);

                viewLocations = viewLocations
                    .Concat(validViewLocationFormats)
                    .Concat(validAreaViewLocationFormats);
            }

            return viewLocations;
        }

        private List<string> GetViewLocationFormats(ILocationFormatProvider locationFormatProvider)
        {
            var formats = locationFormatProvider.ViewLocationFormats.ToList();
            formats.AddRange(locationFormatProvider.PartialViewLocationFormats);
            formats.AddRange(locationFormatProvider.MasterLocationFormats);
            return formats;
        }

        private List<string> GetAreaViewLocationFormats(ILocationFormatProvider locationFormatProvider)
        {
            var formats = locationFormatProvider.AreaViewLocationFormats.ToList();
            formats.AddRange(locationFormatProvider.AreaPartialViewLocationFormats);
            formats.AddRange(locationFormatProvider.AreaMasterLocationFormats);
            return formats;
        }

        private static DefaultControllerFactory CreateControllerFactory()
        {
            var propertyActivators = new IControllerPropertyActivator[]
            {
                new DefaultControllerPropertyActivator(),
            };

            return new DefaultControllerFactory(
                new DefaultControllerActivator(new TypeActivatorCache()),
                propertyActivators);
        }
    }
}