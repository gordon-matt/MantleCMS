using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement
{
    public static class ViewComponentHelperExtensions
    {
        public static async Task<IHtmlContent> AutoBreadcrumbsAsync(this IViewComponentHelper component, string templateViewName)
        {
            return await component.InvokeAsync("AutoBreadcrumbs", new
            {
                templateViewName = templateViewName
            });
        }

        public static async Task<IHtmlContent> AutoMenuAsync(this IViewComponentHelper component, string templateViewName, bool includeHomePageLink = true)
        {
            return await component.InvokeAsync("AutoMenu", new
            {
                templateViewName = templateViewName,
                includeHomePageLink = includeHomePageLink
            });
        }

        public static async Task<IHtmlContent> AutoSubMenuAsync(this IViewComponentHelper component, string templateViewName)
        {
            return await component.InvokeAsync("AutoSubMenu", new
            {
                templateViewName = templateViewName
            });
        }

        public static async Task<IHtmlContent> ContentZoneAsync(this IViewComponentHelper component, string zoneName, bool renderAsWidgets = false, WidgetColumns widgetColumns = WidgetColumns.Default)
        {
            return await component.InvokeAsync("ContentBlocksByZone", new
            {
                zoneName = zoneName,
                renderAsWidgets = renderAsWidgets,
                widgetColumns = widgetColumns
            });
        }

        public static async Task<IHtmlContent> EntityTypeContentZoneAsync(this IViewComponentHelper component, string zoneName, string entityType, object entityId, bool renderAsWidgets = false, WidgetColumns widgetColumns = WidgetColumns.Default)
        {
            return await component.InvokeAsync("EntityTypeContentBlocksByZone", new
            {
                entityType = entityType,
                entityId = entityId,
                zoneName = zoneName,
                renderAsWidgets = renderAsWidgets,
                widgetColumns = widgetColumns
            });
        }

        public static async Task<IHtmlContent> MenuAsync(this IViewComponentHelper component, string menuName, string templateViewName, bool filterByUrl = false)
        {
            return await component.InvokeAsync("Menu", new
            {
                name = menuName,
                templateViewName = templateViewName,
                filterByUrl = filterByUrl
            });
        }
    }
}