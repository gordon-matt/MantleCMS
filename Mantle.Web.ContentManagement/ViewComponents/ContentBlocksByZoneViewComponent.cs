using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.ViewComponents
{
    [ViewComponent(Name = "ContentBlocksByZone")]
    public class ContentBlocksByZoneViewComponent : ViewComponent
    {
        private readonly IEnumerable<IContentBlockProvider> providers;

        public ContentBlocksByZoneViewComponent(IEnumerable<IContentBlockProvider> providers)
        {
            this.providers = providers;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            string zoneName,
            bool renderAsWidgets = false,
            WidgetColumns widgetColumns = WidgetColumns.Default)
        {
            var contentBlocks = providers
                .SelectMany(x => x.GetContentBlocks(zoneName))
                .ToList();

            ViewBag.RenderAsWidgets = renderAsWidgets;
            ViewBag.WidgetColumns = widgetColumns;

            //var viewEngineResult = ViewEngines.Engines.FindView(ControllerContext, "ContentBlocksByZone", null);

            //// If someone has provided a custom template (see LocationFormatProvider)
            //if (viewEngineResult.View != null)
            //{
            //    return View(contentBlocks);
            //}

            return View(contentBlocks);
        }
    }
}