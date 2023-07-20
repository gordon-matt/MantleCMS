using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.ViewComponents
{
    [ViewComponent(Name = "EntityTypeContentBlocksByZone")]
    public class EntityTypeContentBlocksByZoneViewComponent : ViewComponent
    {
        private readonly IEnumerable<IEntityTypeContentBlockProvider> providers;

        public EntityTypeContentBlocksByZoneViewComponent(IEnumerable<IEntityTypeContentBlockProvider> providers)
        {
            this.providers = providers;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            string zoneName,
            string entityType,
            object entityId,
            bool renderAsWidgets = false,
            WidgetColumns widgetColumns = WidgetColumns.Default)
        {
            var contentBlocks = providers
                .SelectMany(x => x.GetContentBlocks(zoneName, entityType, entityId.ToString()))
                .ToList();

            ViewBag.RenderAsWidgets = renderAsWidgets;
            ViewBag.WidgetColumns = widgetColumns;

            //var viewEngineResult = ViewEngines.Engines.FindView(ControllerContext, "ContentBlocksByZone", null);

            //// If someone has provided a custom template (see LocationFormatProvider)
            //if (viewEngineResult.View != null)
            //{
            //    return View("ContentBlocksByZone", contentBlocks);
            //}

            return View(contentBlocks);
        }
    }
}