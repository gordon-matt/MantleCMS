using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;

namespace Mantle.Web.ContentManagement;

public static class HtmlHelperExtensions
{
    public static MantleCMS<TModel> MantleCMS<TModel>(this IHtmlHelper<TModel> html) where TModel : class
    {
        return new MantleCMS<TModel>(html);
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
        int selectedValue = func(html.ViewData.Model);

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
        int selectedValue = func(html.ViewData.Model);

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
        string selectedValue = func(html.ViewData.Model);

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
        string selectedValue = func(html.ViewData.Model);

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
        string selectedValue = func(html.ViewData.Model);

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
        string selectedValue = func(html.ViewData.Model);

        var selectList = GetZonesSelectList(selectedValue, emptyText);
        return html.DropDownListFor(expression, selectList, htmlAttributes);
    }

    public IEnumerable<SelectListItem> GetBlogCategorySelectList(int? selectedValue = null, string emptyText = null)
    {
        var categoryService = EngineContext.Current.Resolve<IBlogCategoryService>();

        return categoryService.Find()
            .ToSelectList(
                value => value.Id,
                text => text.Name,
                selectedValue,
                emptyText);
    }

    public IEnumerable<SelectListItem> GetBlogTagSelectList(int? selectedValue = null, string emptyText = null)
    {
        var tagService = EngineContext.Current.Resolve<IBlogTagService>();

        return tagService.Find()
            .ToSelectList(
                value => value.Id,
                text => text.Name,
                selectedValue,
                emptyText);
    }

    public IEnumerable<SelectListItem> GetContentBlockTypesSelectList(string selectedValue = null, string emptyText = null)
    {
        var contentBlocks = EngineContext.Current.ResolveAll<IContentBlock>();

        var blockTypes = contentBlocks
            .Select(x => new
            {
                x.Name,
                Type = x.GetTypeFullName()
            })
            .OrderBy(x => x.Name)
            .ToDictionary(k => k.Name, v => v.Type);

        return blockTypes.ToSelectList(
            value => value.Value,
            text => text.Key,
            selectedValue,
            emptyText);
    }

    public IEnumerable<SelectListItem> GetPageTypesSelectList(string selectedValue = null, string emptyText = null)
    {
        var repository = EngineContext.Current.Resolve<IRepository<PageType>>();

        using var connection = repository.OpenConnection();
        return connection.Query()
            .OrderBy(x => x.Name)
            .ToList()
            .ToSelectList(
                value => value.Id,
                text => text.Name,
                selectedValue,
                emptyText);
    }

    public IEnumerable<SelectListItem> GetTopLevelPagesSelectList(string selectedValue = null, string emptyText = null)
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

    public IEnumerable<SelectListItem> GetZonesSelectList(string selectedValue = null, string emptyText = null)
    {
        var zoneService = EngineContext.Current.Resolve<IZoneService>();

        using var connection = zoneService.OpenConnection();
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