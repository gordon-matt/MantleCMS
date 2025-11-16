namespace Mantle.Web.ContentManagement;

public static class ViewComponentHelperExtensions
{
    extension(IViewComponentHelper component)
    {
        public async Task<IHtmlContent> AutoBreadcrumbsAsync(string templateViewName) =>
            await component.InvokeAsync("AutoBreadcrumbs", new { templateViewName });

        public async Task<IHtmlContent> AutoMenuAsync(string templateViewName, bool includeHomePageLink = true) =>
            await component.InvokeAsync("AutoMenu", new { templateViewName, includeHomePageLink });

        public async Task<IHtmlContent> AutoSubMenuAsync(string templateViewName) =>
            await component.InvokeAsync("AutoSubMenu", new { templateViewName });

        public async Task<IHtmlContent> ContentZoneAsync(
            string zoneName, bool renderAsWidgets = false, WidgetColumns widgetColumns = WidgetColumns.Default) =>
            await component.InvokeAsync("ContentBlocksByZone", new { zoneName, renderAsWidgets, widgetColumns });

        public async Task<IHtmlContent> EntityTypeContentZoneAsync(
            string zoneName, string entityType, object entityId, bool renderAsWidgets = false, WidgetColumns widgetColumns = WidgetColumns.Default) =>
            await component.InvokeAsync("EntityTypeContentBlocksByZone", new { entityType, entityId, zoneName, renderAsWidgets, widgetColumns });

        public async Task<IHtmlContent> MenuAsync(
            string menuName, string templateViewName, bool filterByUrl = false) =>
            await component.InvokeAsync("Menu", new { name = menuName, templateViewName, filterByUrl });
    }
}