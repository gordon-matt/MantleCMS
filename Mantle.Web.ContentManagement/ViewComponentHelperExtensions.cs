namespace Mantle.Web.ContentManagement;

public static class ViewComponentHelperExtensions
{
    public static async Task<IHtmlContent> AutoBreadcrumbsAsync(this IViewComponentHelper component, string templateViewName) =>
        await component.InvokeAsync("AutoBreadcrumbs", new
        {
            templateViewName
        });

    public static async Task<IHtmlContent> AutoMenuAsync(this IViewComponentHelper component, string templateViewName, bool includeHomePageLink = true) =>
        await component.InvokeAsync("AutoMenu", new
        {
            templateViewName,
            includeHomePageLink
        });

    public static async Task<IHtmlContent> AutoSubMenuAsync(this IViewComponentHelper component, string templateViewName) =>
        await component.InvokeAsync("AutoSubMenu", new
        {
            templateViewName
        });

    public static async Task<IHtmlContent> ContentZoneAsync(
        this IViewComponentHelper component, string zoneName, bool renderAsWidgets = false, WidgetColumns widgetColumns = WidgetColumns.Default) =>
        await component.InvokeAsync("ContentBlocksByZone", new
        {
            zoneName,
            renderAsWidgets,
            widgetColumns
        });

    public static async Task<IHtmlContent> EntityTypeContentZoneAsync(
        this IViewComponentHelper component, string zoneName, string entityType, object entityId, bool renderAsWidgets = false, WidgetColumns widgetColumns = WidgetColumns.Default) =>
        await component.InvokeAsync("EntityTypeContentBlocksByZone", new
        {
            entityType,
            entityId,
            zoneName,
            renderAsWidgets,
            widgetColumns
        });

    public static async Task<IHtmlContent> MenuAsync(
        this IViewComponentHelper component, string menuName, string templateViewName, bool filterByUrl = false) =>
        await component.InvokeAsync("Menu", new
        {
            name = menuName,
            templateViewName,
            filterByUrl
        });
}