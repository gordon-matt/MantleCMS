using System.Collections.Generic;
using System.Linq;
using Mantle.Infrastructure;
using Mantle.Web.Configuration;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Mantle.Web.Mvc.Razor
{
    public class TenantViewLocationExpander : MantleViewLocationExpander
    {
        private const string THEME_KEY = "theme";

        public override void PopulateValues(ViewLocationExpanderContext context)
        {
            base.PopulateValues(context);
            //context.Values[THEME_KEY] = context.ActionContext.HttpContext.GetTenant<Tenant>()?.Theme;
            var siteSettings = EngineContext.Current.Resolve<SiteSettings>();
            context.Values[THEME_KEY] = siteSettings.DefaultTheme;
        }

        public override IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            viewLocations = base.ExpandViewLocations(context, viewLocations);

            if (context.Values.TryGetValue(THEME_KEY, out string theme))
            {
                var locations = new HashSet<string>();
                foreach (string location in viewLocations)
                {
                    if (location.StartsWith("~"))
                    {
                        locations.Add($"~/Themes/{theme}{location.Replace("~", string.Empty)}");
                    }
                    else
                    {
                        locations.Add($"/Themes/{theme}{location}");
                    }

                    locations.Add(location);
                }
                return locations;
            }

            return viewLocations;
        }
    }
}