namespace Mantle.Web.Mvc;

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
    #region Image Map

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

    #endregion Image Map

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

    public static MantleUI<TModel> MantleUI<TModel>(this IHtmlHelper<TModel> htmlHelper, IMantleUIProvider provider = null)
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