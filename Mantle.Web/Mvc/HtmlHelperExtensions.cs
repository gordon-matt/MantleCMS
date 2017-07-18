using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Mantle.Collections;
using Mantle.ComponentModel;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Security.Membership;
using Mantle.Tenants.Services;
using Mantle.Threading;
using Mantle.Web.Collections;
using Mantle.Web.Mvc.Controls;
using Mantle.Web.Mvc.MantleUI;
using Mantle.Web.Mvc.MantleUI.Providers;
using Mantle.Web.Mvc.Rendering;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Mantle.Web.Mvc
{
    public enum PageTarget : byte
    {
        Default = 0,
        Blank = 1,
        Parent = 2,
        Self = 3,
        Top = 4
    }

    public static class HtmlHelperExtensions
    {
        #region Images

        public static IHtmlContent EmbeddedImage(this IHtmlHelper helper, Assembly assembly, string resourceName, string alt, object htmlAttributes = null)
        {
            string base64 = string.Empty;
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var memoryStream = new MemoryStream())
            {
                resourceStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                base64 = Convert.ToBase64String(memoryStream.ToArray());
            }

            return Image(helper, string.Concat("data:image/jpg;base64,", base64), alt, htmlAttributes);
        }

        public static IHtmlContent Image(this IHtmlHelper helper, string src, string alt, object htmlAttributes = null)
        {
            var urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>();
            var actionContextAccessor = EngineContext.Current.Resolve<IActionContextAccessor>();
            var urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            var builder = new TagBuilder("img");
            builder.TagRenderMode = TagRenderMode.SelfClosing;
            builder.MergeAttribute("src", urlHelper.Content(src));

            if (!string.IsNullOrEmpty(alt))
            {
                builder.MergeAttribute("alt", alt);
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return new HtmlString(builder.Build());
        }

        public static IHtmlContent ImageLink(this IHtmlHelper helper, string src, string alt, string href, object aHtmlAttributes = null, object imgHtmlAttributes = null, PageTarget target = PageTarget.Default)
        {
            var builder = new TagBuilder("a");
            builder.MergeAttribute("href", href);

            switch (target)
            {
                case PageTarget.Blank: builder.MergeAttribute("target", "_blank"); break;
                case PageTarget.Parent: builder.MergeAttribute("target", "_parent"); break;
                case PageTarget.Self: builder.MergeAttribute("target", "_self"); break;
                case PageTarget.Top: builder.MergeAttribute("target", "_top"); break;
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(aHtmlAttributes));

            var img = helper.Image(src, alt, imgHtmlAttributes);

            builder.InnerHtml.AppendHtml(img.ToString());

            return new HtmlString(builder.Build());
        }

        public static IHtmlContent Map(this IHtmlHelper helper, string name, ImageMapHotSpot[] hotSpots)
        {
            return helper.Map(name, name, hotSpots);
        }

        public static IHtmlContent Map(this IHtmlHelper helper, string name, string id, ImageMapHotSpot[] hotSpots)
        {
            var map = new ImageMap
            {
                ID = id,
                Name = name,
                HotSpots = hotSpots
            };

            return new HtmlString(map.ToString());
        }

        #endregion Images

        #region Html Link

        public static IHtmlContent EmailLink(this IHtmlHelper helper, string emailAddress)
        {
            return helper.Link(string.Concat("mailto:", emailAddress));
        }

        public static IHtmlContent Link(this IHtmlHelper helper, string href, PageTarget target = PageTarget.Default)
        {
            return helper.Link(href, href, target);
        }

        public static IHtmlContent Link(this IHtmlHelper helper, string linkText, string href, PageTarget target = PageTarget.Default)
        {
            return helper.Link(linkText, href, null, target);
        }

        public static IHtmlContent Link(this IHtmlHelper helper, string linkText, string href, object htmlAttributes, PageTarget target = PageTarget.Default)
        {
            var builder = new TagBuilder("a");
            builder.MergeAttribute("href", href);
            builder.InnerHtml.Append(linkText);

            switch (target)
            {
                case PageTarget.Blank: builder.MergeAttribute("target", "_blank"); break;
                case PageTarget.Parent: builder.MergeAttribute("target", "_parent"); break;
                case PageTarget.Self: builder.MergeAttribute("target", "_self"); break;
                case PageTarget.Top: builder.MergeAttribute("target", "_top"); break;
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return new HtmlString(builder.Build());
        }

        public static IHtmlContent Link(this IHtmlHelper helper, string linkText, string href, RouteValueDictionary htmlAttributes, PageTarget target = PageTarget.Default)
        {
            var builder = new TagBuilder("a");
            builder.MergeAttribute("href", href);
            builder.InnerHtml.Append(linkText);

            switch (target)
            {
                case PageTarget.Blank: builder.MergeAttribute("target", "_blank"); break;
                case PageTarget.Parent: builder.MergeAttribute("target", "_parent"); break;
                case PageTarget.Self: builder.MergeAttribute("target", "_self"); break;
                case PageTarget.Top: builder.MergeAttribute("target", "_top"); break;
            }

            builder.MergeAttributes(htmlAttributes);

            return new HtmlString(builder.Build());
        }

        #endregion Html Link

        #region NumbersDropDown

        public static IHtmlContent NumbersDropDown(this IHtmlHelper html, string name, int min, int max, int? selected = null, string emptyText = null, object htmlAttributes = null)
        {
            var numbers = new List<int>();
            for (int i = min; i <= max; i++)
            {
                numbers.Add(i);
            }

            var selectList = numbers.ToSelectList(value => value, text => text.ToString(), selected, emptyText);

            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public static IHtmlContent NumbersDropDownFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int min, int max, string emptyText = null, object htmlAttributes = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var numbers = new List<int>();
            for (int i = min; i <= max; i++)
            {
                numbers.Add(i);
            }

            var selectList = numbers.ToSelectList(value => value, text => text.ToString(), selectedValue, emptyText);

            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        #endregion NumbersDropDown

        #region Other

        public static IHtmlContent FileUpload(this IHtmlHelper html, string name, object htmlAttributes = null)
        {
            var builder = new TagBuilder("input");
            builder.TagRenderMode = TagRenderMode.SelfClosing;
            builder.MergeAttribute("type", "file");

            if (!string.IsNullOrEmpty(name))
            {
                builder.MergeAttribute("name", name);
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return new HtmlString(builder.Build());
        }

        public static IHtmlContent JsonObject<TEntity>(this IHtmlHelper html, string name, TEntity item)
        {
            return new HtmlString(string.Format("var {0} = {1};", name, item.ToJson()));
        }

        #endregion Other

        public static IHtmlContent CheckBoxList(
            this IHtmlHelper html,
            string name,
            IEnumerable<SelectListItem> selectList,
            IEnumerable<string> selectedValues,
            byte numberOfColumns = 1,
            object labelHtmlAttributes = null,
            object checkboxHtmlAttributes = null)
        {
            string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            string fullHtmlFieldId = html.ViewData.TemplateInfo.GetFullHtmlFieldId(name);

            var values = new List<string>();
            if (selectedValues != null)
            {
                values.AddRange(selectedValues);
            }

            if (selectList.IsNullOrEmpty())
            {
                return HtmlString.Empty;
            }

            int index = 0;

            var sb = new StringBuilder();

            bool groupByCategory = (selectList.First() is ExtendedSelectListItem);

            if (groupByCategory)
            {
                var items = selectList.Cast<ExtendedSelectListItem>().ToList();
                var groups = items.GroupBy(x => x.Category);

                foreach (var @group in groups)
                {
                    sb.AppendFormat(@"<label class=""checkbox-list-group-label"">{0}</label>", group.Key);

                    foreach (var item in group)
                    {
                        var isChecked = values.Contains(item.Value);

                        var tagBuilder = new FluentTagBuilder("label")
                            .MergeAttribute("for", fullHtmlFieldName)
                            .MergeAttributes(labelHtmlAttributes)
                            .StartTag("input", TagRenderMode.SelfClosing)
                                .MergeAttribute("type", "checkbox")
                                .MergeAttribute("name", fullHtmlFieldName)
                                .MergeAttribute("id", fullHtmlFieldId + "_" + index)
                                .MergeAttribute("value", item.Value);

                        if (isChecked)
                        {
                            tagBuilder = tagBuilder.MergeAttribute("checked", "checked");
                        }

                        if (checkboxHtmlAttributes != null)
                        {
                            tagBuilder = tagBuilder.MergeAttributes(checkboxHtmlAttributes);
                        }

                        tagBuilder = tagBuilder.EndTag(); //end checkbox
                        tagBuilder = tagBuilder
                            .StartTag("span")
                                .SetInnerHtml(item.Text)
                            .EndTag();

                        sb.Append(tagBuilder.ToString());
                        index++;
                    }
                }
            }
            else
            {
                var rows = (int)Math.Ceiling((selectList.Count() * 1d) / numberOfColumns);
                var columnWidth = (int)Math.Ceiling(12d / numberOfColumns);

                for (var i = 0; i < numberOfColumns; i++)
                {
                    var items = selectList.Skip(i * rows).Take(rows);
                    sb.AppendFormat("<div class=\"col-md-{0}\">", columnWidth);

                    foreach (var item in items)
                    {
                        var isChecked = values.Contains(item.Value);

                        var tagBuilder = new FluentTagBuilder("label")
                            .MergeAttribute("for", fullHtmlFieldName)
                            .MergeAttributes(labelHtmlAttributes)
                            .StartTag("input", TagRenderMode.SelfClosing)
                                .MergeAttribute("type", "checkbox")
                                .MergeAttribute("name", fullHtmlFieldName)
                                .MergeAttribute("id", fullHtmlFieldId + "_" + index)
                                .MergeAttribute("value", item.Value);

                        if (isChecked)
                        {
                            tagBuilder = tagBuilder.MergeAttribute("checked", "checked");
                        }

                        if (checkboxHtmlAttributes != null)
                        {
                            tagBuilder = tagBuilder.MergeAttributes(checkboxHtmlAttributes);
                        }

                        tagBuilder = tagBuilder.EndTag(); //end checkbox
                        tagBuilder = tagBuilder
                            .StartTag("span")
                                .SetInnerHtml(item.Text)
                            .EndTag();

                        sb.Append(tagBuilder.ToString());
                        index++;
                    }

                    sb.Append("</div>");
                }
            }

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent CheckBoxListFor<TModel, TProperty>(
            this IHtmlHelper<TModel> html,
            Expression<Func<TModel, IEnumerable<TProperty>>> expression,
            IEnumerable<SelectListItem> selectList,
            byte numberOfColumns = 1,
            object labelHtmlAttributes = null,
            object checkboxHtmlAttributes = null) where TModel : class
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string fullHtmlFieldId = html.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName);

            var func = expression.Compile();
            var selectedValues = func(html.ViewData.Model);

            var values = new List<string>();
            if (selectedValues != null)
            {
                values.AddRange(selectedValues.Select(x => Convert.ToString(x)));
            }

            if (selectList == null)
            {
                throw new ArgumentNullException(nameof(selectList));
            }

            int index = 0;

            var sb = new StringBuilder();

            bool groupByCategory = (selectList.First() is ExtendedSelectListItem);

            if (groupByCategory)
            {
                var items = selectList.Cast<ExtendedSelectListItem>().ToList();
                var groups = items.GroupBy(x => x.Category);

                foreach (var @group in groups)
                {
                    sb.AppendFormat("<strong>{0}</strong>", group.Key);

                    foreach (var item in group)
                    {
                        var isChecked = values.Contains(item.Value);

                        var tagBuilder = new FluentTagBuilder("label")
                            .MergeAttribute("for", fullHtmlFieldName)
                            .MergeAttributes(labelHtmlAttributes)
                            .StartTag("input", TagRenderMode.SelfClosing)
                                .MergeAttribute("type", "checkbox")
                                .MergeAttribute("name", fullHtmlFieldName)
                                .MergeAttribute("id", fullHtmlFieldId + "_" + index)
                                .MergeAttribute("value", item.Value);

                        if (isChecked)
                        {
                            tagBuilder = tagBuilder.MergeAttribute("checked", "checked");
                        }

                        if (checkboxHtmlAttributes != null)
                        {
                            tagBuilder = tagBuilder.MergeAttributes(checkboxHtmlAttributes);
                        }

                        tagBuilder = tagBuilder.EndTag(); //end checkbox
                        tagBuilder = tagBuilder
                            .StartTag("span")
                                .SetInnerHtml(item.Text)
                            .EndTag();

                        sb.Append(tagBuilder.ToString());
                        index++;
                    }
                }
            }
            else
            {
                var rows = (int)Math.Ceiling((selectList.Count() * 1d) / numberOfColumns);
                var columnWidth = (int)Math.Ceiling(12d / numberOfColumns);

                for (var i = 0; i < numberOfColumns; i++)
                {
                    var items = selectList.Skip(i * rows).Take(rows);
                    sb.AppendFormat("<div class=\"col-md-{0}\">", columnWidth);

                    foreach (var item in items)
                    {
                        var isChecked = values.Contains(item.Value);

                        var tagBuilder = new FluentTagBuilder("label")
                            .MergeAttribute("for", fullHtmlFieldName)
                            .MergeAttributes(labelHtmlAttributes)
                            .StartTag("input", TagRenderMode.SelfClosing)
                                .MergeAttribute("type", "checkbox")
                                .MergeAttribute("name", fullHtmlFieldName)
                                .MergeAttribute("id", fullHtmlFieldId + "_" + index)
                                .MergeAttribute("value", item.Value);

                        if (isChecked)
                        {
                            tagBuilder = tagBuilder.MergeAttribute("checked", "checked");
                        }

                        if (checkboxHtmlAttributes != null)
                        {
                            tagBuilder = tagBuilder.MergeAttributes(checkboxHtmlAttributes);
                        }

                        tagBuilder = tagBuilder.EndTag(); //end checkbox
                        tagBuilder = tagBuilder
                            .StartTag("span")
                                .SetInnerHtml(item.Text)
                            .EndTag();

                        sb.Append(tagBuilder.ToString());
                        index++;
                    }

                    sb.Append("</div>");
                }
            }

            return new HtmlString(sb.ToString());
        }

        // PROBLEM: https://github.com/dotnet/corefx/issues/1669
        //public static IHtmlContent CulturesDropDownList<TModel>(this IHtmlHelper<TModel> html, string name, string selectedValue = null, object htmlAttributes = null, string emptyText = null)
        //{
        //    var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        //    var selectList = cultures
        //        .OrderBy(x => x.DisplayName)
        //        .ToSelectList(
        //            value => value.Name,
        //            text => text.DisplayName,
        //            selectedValue,
        //            emptyText);

        //    return html.DropDownList(name, selectList, htmlAttributes);
        //}

        //public static IHtmlContent CulturesDropDownListFor<TModel>(this IHtmlHelper<TModel> html, Expression<Func<TModel, string>> expression, object htmlAttributes = null, string emptyText = null)
        //{
        //    var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        //    var func = expression.Compile();
        //    var selectedValue = func(html.ViewData.Model);

        //    var selectList = cultures
        //        .OrderBy(x => x.DisplayName)
        //        .ToSelectList(
        //            value => value.Name,
        //            text => text.DisplayName,
        //            selectedValue,
        //            emptyText);

        //    return html.DropDownListFor(expression, selectList, htmlAttributes);
        //}

        //public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TEnum>> expression, string emptyText = null, object htmlAttributes = null) where TEnum : struct
        //{
        //    var func = expression.Compile();
        //    var selectedValue = func(html.ViewData.Model);

        //    var selectList = EnumExtensions.ToSelectList<TEnum>(selectedValue, emptyText);
        //    return html.DropDownListFor(expression, selectList, htmlAttributes);
        //}

        public static IHtmlContent EnumMultiDropDownListFor<TModel, TEnum>(this IHtmlHelper<TModel> html, Expression<Func<TModel, IEnumerable<TEnum>>> expression, string emptyText = null, object htmlAttributes = null) where TEnum : struct
        {
            var func = expression.Compile();
            var selectedValues = func(html.ViewData.Model);

            var parsedSelectedValues = Enumerable.Empty<long>();

            if (selectedValues != null)
            {
                parsedSelectedValues = selectedValues.Select(x => Convert.ToInt64(x));
            }

            var multiSelectList = EnumExtensions.ToMultiSelectList<TEnum>(parsedSelectedValues, emptyText);

            return html.ListBoxFor(expression, multiSelectList, htmlAttributes);
        }

        public static IHtmlContent HelpText(this IHtmlHelper html, string helpText, object htmlAttributes = null)
        {
            var tagBuilder = new FluentTagBuilder("p")
                .AddCssClass("help-block")
                .MergeAttributes(htmlAttributes)
                .SetInnerHtml(helpText);

            return new HtmlString(tagBuilder.ToString());
        }

        public static IHtmlContent HelpTextFor<TModel, TProperty>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var memberExpression = expression.Body as MemberExpression;
            var propertyInfo = (memberExpression.Member as PropertyInfo);
            var attribute = propertyInfo.GetCustomAttributes().OfType<LocalizedHelpTextAttribute>().FirstOrDefault();

            if (attribute == null)
            {
                return HtmlString.Empty;
            }

            var tagBuilder = new FluentTagBuilder("p")
                .AddCssClass("help-block")
                .MergeAttributes(htmlAttributes)
                .SetInnerHtml(attribute.HelpText);

            return new HtmlString(tagBuilder.ToString());
        }

        public static Mantle<TModel> Mantle<TModel>(this IHtmlHelper<TModel> html) where TModel : class
        {
            return new Mantle<TModel>(html);
        }

        public static MantleUI<TModel> MantleUI<TModel>(this IHtmlHelper<TModel> htmlHelper, IKoreUIProvider provider = null)
        {
            if (provider != null)
            {
                return new MantleUI<TModel>(htmlHelper, provider);
            }

            string areaName = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            if (!string.IsNullOrEmpty(areaName) && MantleUISettings.AreaUIProviders.ContainsKey(areaName))
            {
                return new MantleUI<TModel>(htmlHelper, MantleUISettings.AreaUIProviders[areaName]);
            }
            return new MantleUI<TModel>(htmlHelper);
        }

        //TODO: Test
        public static IHtmlContent Table<T>(this IHtmlHelper html, IEnumerable<T> items, object htmlAttributes = null)
        {
            var builder = new FluentTagBuilder("table")
                .MergeAttributes(htmlAttributes)
                .StartTag("thead")
                    .StartTag("tr");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                builder = builder.StartTag("th").SetInnerHtml(property.Name).EndTag();
            }

            builder
                .EndTag() // </tr>
                .EndTag() // </thead>
                .StartTag("tbody");

            foreach (var item in items)
            {
                builder = builder.StartTag("tr");
                foreach (var property in properties)
                {
                    string value = property.GetValue(item).ToString();
                    builder = builder.StartTag("td").SetInnerHtml(value).EndTag();
                }
                builder = builder.EndTag(); // </tr>
            }
            builder = builder.EndTag(); // </tbody>

            return new HtmlString(builder.ToString());
        }

        ///// <summary>
        ///// Create an HTML tree from a recursive collection of items
        ///// </summary>
        //public static TreeView<T> TreeView<T>(this IHtmlHelper html, IEnumerable<T> items)
        //{
        //    return new TreeView<T>(html, items);
        //}
    }

    public class Mantle<TModel>
        where TModel : class
    {
        private readonly IHtmlHelper<TModel> html;

        internal Mantle(IHtmlHelper<TModel> html)
        {
            this.html = html;
        }

        public IHtmlContent LanguagesDropDownList(string name, string selectedValue = null, object htmlAttributes = null, string emptyText = null, bool includeInvariant = false, string invariantText = null)
        {
            var selectList = GetLanguages(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        /// <summary>
        /// Returns an HTML select element populated with the languages currently specified in the admin area as active
        /// </summary>
        /// <param name="expression"> An expression that identifies the property to use. This property should contain a culture code value</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns></returns>
        public IHtmlContent LanguagesDropDownListFor(Expression<Func<TModel, string>> expression, object htmlAttributes = null, string emptyText = null, bool includeInvariant = false, string invariantText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = GetLanguages(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public IHtmlContent PermissionsCheckBoxList(
            string name,
            IEnumerable<string> selectedPermissionIds,
            object labelHtmlAttributes = null,
            object checkboxHtmlAttributes = null)
        {
            var membershipService = EngineContext.Current.Resolve<IMembershipService>();
            var permissionProviders = EngineContext.Current.ResolveAll<IPermissionProvider>();
            var permissions = permissionProviders.SelectMany(x => x.GetPermissions()).ToList();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var allPermissions = AsyncHelper.RunSync(() => membershipService.GetAllPermissions(workContext.CurrentTenant.Id)).ToHashSet();
            var T = EngineContext.Current.Resolve<IStringLocalizer>();

            #region First check if all permissions are in the DB

            foreach (var permission in permissions)
            {
                if (!allPermissions.Any(x => x.Name == permission.Name))
                {
                    var newPermission = new MantlePermission
                    {
                        Name = permission.Name,
                        Category = string.IsNullOrEmpty(permission.Category) ? T[MantleWebLocalizableStrings.General.Miscellaneous] : permission.Category,
                        Description = permission.Description
                    };

                    newPermission.TenantId = workContext.CurrentTenant.Id;
                    membershipService.InsertPermission(newPermission);
                    allPermissions.Add(newPermission);
                }
            }

            #endregion First check if all permissions are in the DB

            var selectList = new List<ExtendedSelectListItem>();
            foreach (var categoryGroup in allPermissions.OrderBy(x => x.Category, new PermissionComparer(StringComparer.OrdinalIgnoreCase)).GroupBy(x => x.Category))
            {
                selectList.AddRange(categoryGroup.OrderBy(x => x.Description)
                    .Select(permission => new ExtendedSelectListItem
                    {
                        Category = permission.Category,
                        Text = permission.Description,
                        Value = permission.Id
                    }));
            }

            return html.CheckBoxList(
                name,
                selectList,
                selectedPermissionIds,
                labelHtmlAttributes: labelHtmlAttributes,
                checkboxHtmlAttributes: checkboxHtmlAttributes);
        }

        public IHtmlContent RolesCheckBoxList(
            string name,
            IEnumerable<string> selectedRoleIds,
            object labelHtmlAttributes = null,
            object checkboxHtmlAttributes = null)
        {
            var membershipService = EngineContext.Current.Resolve<IMembershipService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var selectList = AsyncHelper.RunSync(() => membershipService.GetAllRoles(workContext.CurrentTenant.Id))
                .ToSelectList(
                    value => value.Id,
                    text => text.Name);

            //TODO: problem when no roles, which happens now because current tenant does not have an admin role.. only NULL tenant has admin role.
            //      need to auto create admin roles for each tenant

            return html.CheckBoxList(name, selectList, selectedRoleIds, labelHtmlAttributes: labelHtmlAttributes, checkboxHtmlAttributes: checkboxHtmlAttributes);
        }

        public IHtmlContent RolesDropDownList(string name, string selectedValue = null, object htmlAttributes = null, string emptyText = null)
        {
            var membershipService = EngineContext.Current.Resolve<IMembershipService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var selectList = AsyncHelper.RunSync(() => membershipService.GetAllRoles(workContext.CurrentTenant.Id))
                .ToSelectList(
                    value => value.Id,
                    text => text.Name,
                    selectedValue,
                    emptyText);

            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public IHtmlContent RolesDropDownListFor(Expression<Func<TModel, string>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var membershipService = EngineContext.Current.Resolve<IMembershipService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var selectList = AsyncHelper.RunSync(() => membershipService.GetAllRoles(workContext.CurrentTenant.Id))
                .ToSelectList(
                    value => value.Id,
                    text => text.Name,
                    selectedValue,
                    emptyText);

            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        /// <summary>
        /// Returns an HTML select element populated with the themes currently available
        /// </summary>
        /// <param name="expression"> An expression that identifies the property to use. This property should contain a theme name.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns></returns>
        public IHtmlContent ThemesDropDownListFor(Expression<Func<TModel, string>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var themeProvider = EngineContext.Current.Resolve<IThemeProvider>();
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = themeProvider.GetThemeConfigurations()
                .ToSelectList(
                    value => value.ThemeName,
                    text => text.ThemeName,
                    selectedValue,
                    emptyText);

            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public IHtmlContent TenantsCheckBoxList(
            string name,
            IEnumerable<string> selectedTenantIds,
            object labelHtmlAttributes = null,
            object checkboxHtmlAttributes = null)
        {
            var selectList = GetTenantsMultiSelectList(selectedTenantIds);
            return html.CheckBoxList(name, selectList, selectedTenantIds, labelHtmlAttributes: labelHtmlAttributes, checkboxHtmlAttributes: checkboxHtmlAttributes);
        }

        public IHtmlContent TenantsDropDownList(string name, string selectedValue = null, object htmlAttributes = null, string emptyText = null)
        {
            var selectList = GetTenantsSelectList(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        private static IEnumerable<SelectListItem> GetTenantsMultiSelectList(IEnumerable<string> selectedValues = null, string emptyText = null)
        {
            var service = EngineContext.Current.Resolve<ITenantService>();

            using (var connection = service.OpenConnection())
            {
                return connection.Query()
                    .OrderBy(x => x.Name)
                    .ToList()
                    .ToMultiSelectList(
                        value => value.Id,
                        text => text.Name,
                        selectedValues,
                        emptyText);
            }
        }
        
        private static IEnumerable<SelectListItem> GetTenantsSelectList(string selectedValue = null, string emptyText = null)
        {
            var service = EngineContext.Current.Resolve<ITenantService>();

            using (var connection = service.OpenConnection())
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

        //TODO: Test this (should be different per tenant). Use the "UsePerTenant" feature in SaaSKit when registering RequestLocalizationOptions.
        private static IEnumerable<SelectListItem> GetLanguages(string selectedValue = null, string emptyText = null, bool includeInvariant = false, string invariantText = null)
        {
            var requestLocalizationOptions = EngineContext.Current.Resolve<IOptions<RequestLocalizationOptions>>();
            var languages = requestLocalizationOptions.Value.SupportedCultures.Select(x => new
            {
                CultureCode = x.Name,
                Name = x.NativeName
            }).ToList();

            if (includeInvariant)
            {
                if (string.IsNullOrEmpty(invariantText))
                {
                    languages.Insert(0, new { CultureCode = string.Empty, Name = "[ Invariant ]" });
                }
                else
                {
                    languages.Insert(0, new { CultureCode = string.Empty, Name = invariantText });
                }
            }

            return languages
                .ToSelectList(
                    value => value.CultureCode,
                    text => text.Name,
                    selectedValue,
                    emptyText);
        }

        //private static IEnumerable<SelectListItem> GetLanguages(string selectedValue = null, string emptyText = null, bool includeInvariant = false, string invariantText = null)
        //{
        //    var languageManager = EngineContext.Current.Resolve<ILanguageManager>();
        //    var workContext = EngineContext.Current.Resolve<IWorkContext>();
        //    var languages = languageManager.GetActiveLanguages(workContext.CurrentTenant.Id).ToList();

        //    if (includeInvariant)
        //    {
        //        if (string.IsNullOrEmpty(invariantText))
        //        {
        //            languages.Insert(0, new Language { CultureCode = null, Name = "[ Invariant ]" });
        //        }
        //        else
        //        {
        //            languages.Insert(0, new Language { CultureCode = null, Name = invariantText });
        //        }
        //    }

        //    return languages
        //        .ToSelectList(
        //            value => value.CultureCode,
        //            text => text.Name,
        //            selectedValue,
        //            emptyText);
        //}

        private class PermissionComparer : IComparer<string>
        {
            private readonly IComparer<string> baseComparer;

            public PermissionComparer(IComparer<string> baseComparer)
            {
                this.baseComparer = baseComparer;
            }

            public int Compare(string x, string y)
            {
                var value = String.Compare(x, y, StringComparison.Ordinal);

                if (value == 0)
                {
                    return 0;
                }

                if (baseComparer.Compare(x, "System") == 0)
                {
                    return -1;
                }

                if (baseComparer.Compare(y, "System") == 0)
                {
                    return 1;
                }

                return value;
            }
        }
    }
}