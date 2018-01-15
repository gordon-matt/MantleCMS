﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mantle.Data;
using Mantle.Infrastructure;
using Mantle.Web.Collections;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Web.ContentManagement
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent AutoBreadcrumbs(this IHtmlHelper html, string templateViewName)
        {
            return html.Action("AutoBreadcrumbs", "Frontend", new
            {
                area = string.Empty,
                templateViewName = templateViewName
            });
        }

        public static IHtmlContent AutoMenu(this IHtmlHelper html, string templateViewName, bool includeHomePageLink = true)
        {
            return html.Action("AutoMenu", "Frontend", new
            {
                area = string.Empty,
                templateViewName = templateViewName,
                includeHomePageLink = includeHomePageLink
            });
        }

        public static IHtmlContent AutoSubMenu(this IHtmlHelper html, string templateViewName)
        {
            return html.Action("AutoSubMenu", "Frontend", new
            {
                area = string.Empty,
                templateViewName = templateViewName
            });
        }

        public static IHtmlContent ContentZone(this IHtmlHelper html, string zoneName, bool renderAsWidgets = false, WidgetColumns widgetColumns = WidgetColumns.Default)
        {
            return html.Action("ContentBlocksByZone", "Frontend", new
            {
                area = string.Empty,
                zoneName = zoneName,
                renderAsWidgets = renderAsWidgets,
                widgetColumns = widgetColumns
            });
        }

        public static IHtmlContent EntityTypeContentZone(this IHtmlHelper html, string zoneName, string entityType, object entityId, bool renderAsWidgets = false, WidgetColumns widgetColumns = WidgetColumns.Default)
        {
            return html.Action("EntityTypeContentBlocksByZone", "Frontend", new
            {
                area = string.Empty,
                entityType = entityType,
                entityId = entityId,
                zoneName = zoneName,
                renderAsWidgets = renderAsWidgets,
                widgetColumns = widgetColumns
            });
        }

        public static MantleCMS<TModel> MantleCMS<TModel>(this IHtmlHelper<TModel> html) where TModel : class
        {
            return new MantleCMS<TModel>(html);
        }

        public static IHtmlContent Menu(this IHtmlHelper html, string menuName, string templateViewName, bool filterByUrl = false)
        {
            return html.Action("Menu", "Frontend", new
            {
                area = string.Empty,
                name = menuName,
                templateViewName = templateViewName,
                filterByUrl = filterByUrl
            });
        }
    }

    public enum WidgetColumns : byte
    {
        Default = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }

    public class MantleCMS<TModel>
        where TModel : class
    {
        private readonly IHtmlHelper<TModel> html;

        internal MantleCMS(IHtmlHelper<TModel> html)
        {
            this.html = html;
        }

        public IHtmlContent BlogCategoryDropDownList(string name, int? selectedValue = null, string emptyText = null, object htmlAttributes = null)
        {
            var selectList = GetBlogCategorySelectList(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public IHtmlContent BlogCategoryDropDownListFor(Expression<Func<TModel, int>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = GetBlogCategorySelectList(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public IHtmlContent BlogTagDropDownList(string name, int? selectedValue = null, string emptyText = null, object htmlAttributes = null)
        {
            var selectList = GetBlogTagSelectList(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public IHtmlContent BlogTagDropDownListFor(Expression<Func<TModel, int>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = GetBlogTagSelectList(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public IHtmlContent ContentBlockTypesDropDownList(string name, string selectedValue = null, string emptyText = null, object htmlAttributes = null)
        {
            var selectList = GetContentBlockTypesSelectList(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public IHtmlContent ContentBlockTypesDropDownListFor(Expression<Func<TModel, string>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = GetContentBlockTypesSelectList(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public IHtmlContent PageTypesDropDownList(string name, string selectedValue = null, string emptyText = null, object htmlAttributes = null)
        {
            var selectList = GetPageTypesSelectList(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public IHtmlContent PageTypesDropDownListFor(Expression<Func<TModel, string>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = GetPageTypesSelectList(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public IHtmlContent TopLevelPagesDropDownList(string name, string selectedValue = null, string emptyText = null, object htmlAttributes = null)
        {
            var selectList = GetTopLevelPagesSelectList(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public IHtmlContent TopLevelPagesDropDownListFor(Expression<Func<TModel, string>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = GetTopLevelPagesSelectList(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public IHtmlContent ZonesDropDownList(string name, string selectedValue = null, string emptyText = null, object htmlAttributes = null)
        {
            var selectList = GetZonesSelectList(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public IHtmlContent ZonesDropDownListFor(Expression<Func<TModel, string>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = GetZonesSelectList(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        private static IEnumerable<SelectListItem> GetBlogCategorySelectList(int? selectedValue = null, string emptyText = null)
        {
            var categoryService = EngineContext.Current.Resolve<IBlogCategoryService>();

            return categoryService.Find()
                .ToSelectList(
                    value => value.Id,
                    text => text.Name,
                    selectedValue,
                    emptyText);
        }

        private static IEnumerable<SelectListItem> GetBlogTagSelectList(int? selectedValue = null, string emptyText = null)
        {
            var tagService = EngineContext.Current.Resolve<IBlogTagService>();

            return tagService.Find()
                .ToSelectList(
                    value => value.Id,
                    text => text.Name,
                    selectedValue,
                    emptyText);
        }

        private static IEnumerable<SelectListItem> GetContentBlockTypesSelectList(string selectedValue = null, string emptyText = null)
        {
            var contentBlocks = EngineContext.Current.ResolveAll<IContentBlock>();

            var blockTypes = contentBlocks
                .Select(x => new
                {
                    x.Name,
                    Type = GetTypeFullName(x.GetType())
                })
                .OrderBy(x => x.Name)
                .ToDictionary(k => k.Name, v => v.Type);

            return blockTypes.ToSelectList(
                value => value.Value,
                text => text.Key,
                selectedValue,
                emptyText);
        }

        private static IEnumerable<SelectListItem> GetPageTypesSelectList(string selectedValue = null, string emptyText = null)
        {
            var repository = EngineContext.Current.Resolve<IRepository<PageType>>();

            using (var connection = repository.OpenConnection())
            {
                return connection.Query()
                    .OrderBy(x => x.Name)
                    .ToList()
                    .ToSelectList(
                        value => value.Id,
                        text => text.Name,
                        selectedValue,
                        emptyText);
            }
        }

        private static IEnumerable<SelectListItem> GetTopLevelPagesSelectList(string selectedValue = null, string emptyText = null)
        {
            var pageService = EngineContext.Current.Resolve<IPageService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            int tenantId = workContext.CurrentTenant.Id;

            return pageService.GetTopLevelPages(tenantId)
                .ToSelectList(
                    value => value.Id,
                    text => text.Name,
                    selectedValue,
                    emptyText);
        }

        private static string GetTypeFullName(Type type)
        {
            return string.Concat(type.FullName, ", ", type.Assembly.GetName().Name);
        }

        private static IEnumerable<SelectListItem> GetZonesSelectList(string selectedValue = null, string emptyText = null)
        {
            var zoneService = EngineContext.Current.Resolve<IZoneService>();

            using (var connection = zoneService.OpenConnection())
            {
                return connection.Query()
                    .OrderBy(x => x.Name)
                    .ToList()
                    .ToSelectList(
                        value => value.Id,
                        text => text.Name,
                        selectedValue,
                        emptyText);
            }
        }
    }
}