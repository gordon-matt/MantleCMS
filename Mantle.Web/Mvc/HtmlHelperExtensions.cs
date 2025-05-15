using Extenso.AspNetCore.Mvc.Html;
using Humanizer;

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
    #region Html Link

    public static IHtmlContent EmailLink(this IHtmlHelper helper, string emailAddress) => helper.Link(string.Concat("mailto:", emailAddress));

    public static IHtmlContent Link(this IHtmlHelper helper, string href, PageTarget target = PageTarget.Default) => helper.Link(href, href, target);

    public static IHtmlContent Link(this IHtmlHelper helper, string linkText, string href, PageTarget target = PageTarget.Default) => helper.Link(linkText, href, null, target);

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
        var propertyInfo = memberExpression.Member as PropertyInfo;
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

    public static Mantle<TModel> Mantle<TModel>(this IHtmlHelper<TModel> html) where TModel : class => new(html);

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

    public IHtmlContent LanguagesDropDownList(
        string name,
        string selectedValue = null,
        object htmlAttributes = null,
        string emptyText = null,
        bool includeInvariant = false,
        string invariantText = null)
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
    public IHtmlContent LanguagesDropDownListFor(
        Expression<Func<TModel, string>> expression,
        object htmlAttributes = null,
        string emptyText = null,
        bool includeInvariant = false,
        string invariantText = null)
    {
        var func = expression.Compile();
        string selectedValue = func(html.ViewData.Model);

        var selectList = GetLanguages(selectedValue, emptyText);
        return html.DropDownListFor(expression, selectList, htmlAttributes);
    }

    public IHtmlContent PermissionsCheckBoxList(
        string name,
        IEnumerable<string> selectedPermissionIds,
        object labelHtmlAttributes = null,
        object checkboxHtmlAttributes = null)
    {
        var membershipService = DependoResolver.Instance.Resolve<IMembershipService>();
        var permissionProviders = DependoResolver.Instance.ResolveAll<IPermissionProvider>();
        var permissions = permissionProviders.SelectMany(x => x.GetPermissions()).ToList();
        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();

        var allPermissions = AsyncHelper.RunSync(() => membershipService.GetAllPermissions(workContext.CurrentTenant.Id)).ToHashSet();
        var T = DependoResolver.Instance.Resolve<IStringLocalizer>();

        #region First check if all permissions are in the DB

        foreach (var permission in permissions)
        {
            if (!allPermissions.Any(x => x.Name == permission.Name))
            {
                var newPermission = new MantlePermission
                {
                    Name = permission.Name,
                    Category = string.IsNullOrEmpty(permission.Category) ? T[MantleWebLocalizableStrings.General.Miscellaneous] : permission.Category,
                    Description = permission.Description,
                    TenantId = workContext.CurrentTenant.Id
                };
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
        var membershipService = DependoResolver.Instance.Resolve<IMembershipService>();
        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();

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
        var membershipService = DependoResolver.Instance.Resolve<IMembershipService>();
        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();

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
        string selectedValue = func(html.ViewData.Model);

        var membershipService = DependoResolver.Instance.Resolve<IMembershipService>();
        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();

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
        var themeProvider = DependoResolver.Instance.Resolve<IThemeProvider>();
        var func = expression.Compile();
        string selectedValue = func(html.ViewData.Model);

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
        var service = DependoResolver.Instance.Resolve<ITenantService>();

        using var connection = service.OpenConnection();
        return connection.Query()
            .OrderBy(x => x.Name)
            .ToList()
            .ToMultiSelectList(
                value => value.Id,
                text => text.Name,
                selectedValues,
                emptyText);
    }

    private static IEnumerable<SelectListItem> GetTenantsSelectList(string selectedValue = null, string emptyText = null)
    {
        var service = DependoResolver.Instance.Resolve<ITenantService>();

        using var connection = service.OpenConnection();
        return connection.Query()
            .OrderBy(x => x.Name)
            .ToList()
            .ToSelectList(
                value => value.Id,
                text => text.Name,
                selectedValue,
                emptyText);
    }

    //TODO: Test this (should be different per tenant). Use the "UsePerTenant" feature in SaaSKit when registering RequestLocalizationOptions.
    private static IEnumerable<SelectListItem> GetLanguages(string selectedValue = null, string emptyText = null, bool includeInvariant = false, string invariantText = null)
    {
        var requestLocalizationOptions = DependoResolver.Instance.Resolve<IOptions<RequestLocalizationOptions>>();
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
    //    var languageManager = DependoResolver.Instance.Resolve<ILanguageManager>();
    //    var workContext = DependoResolver.Instance.Resolve<IWorkContext>();
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

    public async Task<IHtmlContent> EmbeddedPartialAsync(EmbeddedPartialType type, string viewName = null, object model = null)
    {
        var razorViewRenderService = DependoResolver.Instance.Resolve<IRazorViewRenderService>();

        return type switch
        {
            EmbeddedPartialType.ResourceSettings => new HtmlString(await razorViewRenderService.RenderToStringAsync("/Views/Shared/EditorTemplates/_ResourceSettings.cshtml")),
            EmbeddedPartialType.Custom => new HtmlString(await razorViewRenderService.RenderToStringAsync(viewName, model)),
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }

    public IHtmlContent FileManager(string fieldId, FileFilterMode filterMode, int? tenantId = null)
    {
        byte filterModeValue;
        string extensions = null;
        if (filterMode == FileFilterMode.Folder)
        {
            filterModeValue = 2;
            extensions = "&extensions=[&quot;&quot;]";
        }
        else
        {
            filterModeValue = (byte)filterMode;
        }

        if (!tenantId.HasValue)
        {
            var workContext = DependoResolver.Instance.Resolve<IWorkContext>();
            tenantId = workContext.CurrentTenant.Id;
        }

        string src = $@"/filemanager/dialog.php?field_id={fieldId}&rootFolder=Tenant_{tenantId.Value}&type={filterModeValue}{extensions}&relative_url=1&fldr=&ignore_last_position=1";

        var tagBuilder = new FluentTagBuilder("iframe")
            .MergeAttribute("src", src)
            .MergeAttribute("frameborder", 0)
            .MergeAttribute("height", "100%")
            .MergeAttribute("width", "100%")
            .MergeAttribute("style", "overflow:hidden;min-height:600px; height:100%;width:100%");

        return new HtmlString(tagBuilder.ToString());
    }

    public ModalFileManager<TModel> ModalFileManager(string fieldId, FileFilterMode filterMode, string modalTitle, string knockoutBinding = null) => new(html)
    {
        FieldId = fieldId,
        FilterMode = filterMode,
        ModalTitle = modalTitle,
        KnockoutBinding = knockoutBinding
    };

    private class PermissionComparer : IComparer<string>
    {
        private readonly IComparer<string> baseComparer;

        public PermissionComparer(IComparer<string> baseComparer)
        {
            this.baseComparer = baseComparer;
        }

        public int Compare(string x, string y)
        {
            int value = string.Compare(x, y, StringComparison.Ordinal);

            return value == 0 ? 0 : baseComparer.Compare(x, "System") == 0 ? -1 : baseComparer.Compare(y, "System") == 0 ? 1 : value;
        }
    }
}

public enum EmbeddedPartialType : byte
{
    Custom = 0,
    ResourceSettings = 1
}

public enum FileFilterMode : byte
{
    None = 0,
    Image = 1,
    File = 2,
    Video = 3,
    Folder = 4
}

public class ModalFileManager<TModel>
    where TModel : class
{
    private readonly IHtmlHelper<TModel> htmlHelper;

    internal ModalFileManager(IHtmlHelper<TModel> htmlHelper)
    {
        this.htmlHelper = htmlHelper;
    }

    public string FieldId { get; set; }

    public FileFilterMode FilterMode { get; set; }

    public string ModalTitle { get; set; }

    public string KnockoutBinding { get; set; }

    public string ModalId => $"{FieldId}FileManagerModal";

    public IHtmlContent RenderModal()
    {
        string html =
$@"<div class=""modal mantle-file-manager-modal"" tabindex=""-1"" id=""{ModalId}"">
    <div class=""modal-dialog modal-xl"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h5 class=""modal-title"">{ModalTitle}</h5>
                <button type=""button"" class=""btn-close"" onclick=""dismiss{ModalId}();"" aria-label=""Close""></button>
            </div>
            <div class=""modal-body"">
                {htmlHelper.Mantle().FileManager(FieldId, FilterMode).GetString()}
            </div>
        </div>
    </div>
</div>";

        return new HtmlString(html);
    }

    public IHtmlContent RenderScript()
    {
        string variableName = $"{ModalId.Camelize()}Dismissed";
        string knockoutBinding = KnockoutBinding ?? FieldId.Camelize();

        string js =
$@"var {variableName} = false;

function dismiss{ModalId}() {{
    {variableName} = true;
    $('#{ModalId}').modal('hide');
}};

$('#{ModalId}').on('hidden.bs.modal', function () {{
    if (!{variableName}) {{
        const url = `/Media/Uploads/${{$('#{FieldId}').val()}}`;
        {knockoutBinding}(url);
    }}
    {variableName} = false;
}});";

        return new HtmlString(js);
    }

    public IHtmlContent RenderFormField()
    {
        string knockoutBinding = KnockoutBinding ?? FieldId.Camelize();
        string input = htmlHelper.TextBox(FieldId, null, new { @class = "form-control", data_bind = $"value: {knockoutBinding}" }).GetString();

        string html =
$@"<div class=""input-group"">
    {input}
    <span class=""input-group-text"">
        <a data-bs-toggle=""modal"" href=""javascript:void(0);"" data-bs-target=""#{ModalId}"">
            <i class=""fa fa-search""></i>
        </a>
    </span>
</div>";

        return new HtmlString(html);
    }
}