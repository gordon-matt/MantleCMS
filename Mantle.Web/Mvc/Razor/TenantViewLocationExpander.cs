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

            string theme = null;
            if (context.Values.TryGetValue(THEME_KEY, out theme))
            {
                viewLocations = new[]
                {
                    $"/Areas/{{2}}/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                    $"/Areas/{{2}}/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                    $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                    $"/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                }
                .Concat(viewLocations);
            }

            return viewLocations;
        }
    }
}