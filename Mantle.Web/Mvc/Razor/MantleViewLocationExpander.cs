using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Mantle.Web.Mvc.Razor
{
    public class MantleViewLocationExpander : IViewLocationExpander
    {
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

            return viewLocations;
        }
    }
}