using Mantle.Web.Common.Areas.Admin.Regions.Services;

namespace Mantle.Web.Common;

public static class HtmlHelperExtensions
{
    public static MantleCommon<TModel> MantleCommon<TModel>(this HtmlHelper<TModel> html) where TModel : class => new(html);
}

public class MantleCommon<TModel>
    where TModel : class
{
    private readonly HtmlHelper<TModel> html;

    internal MantleCommon(HtmlHelper<TModel> html)
    {
        this.html = html;
    }

    #region Regions

    #region Continents

    public IHtmlContent ContinentDropDownList(string name, int? selectedValue = null, string emptyText = null, object htmlAttributes = null)
    {
        var selectList = GetContinents(selectedValue, emptyText);
        return html.DropDownList(name, selectList, htmlAttributes);
    }

    public IHtmlContent ContinentDropDownListFor(Expression<Func<TModel, int>> expression, object htmlAttributes = null, string emptyText = null)
    {
        var func = expression.Compile();
        int selectedValue = func(html.ViewData.Model);

        var selectList = GetContinents(selectedValue, emptyText);
        return html.DropDownListFor(expression, selectList, htmlAttributes);
    }

    public IHtmlContent ContinentCheckBoxList(
        string name,
        IEnumerable<string> selectedIds,
        object labelHtmlAttributes = null,
        object checkboxHtmlAttributes = null)
    {
        var selectList = GetContinents();

        return html.CheckBoxList(
            name,
            selectList,
            selectedIds,
            labelHtmlAttributes: labelHtmlAttributes,
            checkboxHtmlAttributes: checkboxHtmlAttributes);
    }

    public IHtmlContent ContinentCheckBoxListFor(
        Expression<Func<TModel, IEnumerable<string>>> expression,
        object labelHtmlAttributes = null,
        object checkboxHtmlAttributes = null)
    {
        var selectList = GetContinents();

        return html.CheckBoxListFor(
            expression,
            selectList,
            labelHtmlAttributes: labelHtmlAttributes,
            checkboxHtmlAttributes: checkboxHtmlAttributes);
    }

    #endregion Continents

    #region Countries

    public IHtmlContent CountryDropDownList(string name, int? selectedValue = null, string emptyText = null, object htmlAttributes = null)
    {
        var selectList = GetCountries(selectedValue, emptyText);
        return html.DropDownList(name, selectList, htmlAttributes);
    }

    public IHtmlContent CountryDropDownListFor(Expression<Func<TModel, int>> expression, object htmlAttributes = null, string emptyText = null)
    {
        var func = expression.Compile();
        int selectedValue = func(html.ViewData.Model);

        var selectList = GetCountries(selectedValue, emptyText);
        return html.DropDownListFor(expression, selectList, htmlAttributes);
    }

    public IHtmlContent CountryCheckBoxList(
        string name,
        IEnumerable<string> selectedIds,
        object labelHtmlAttributes = null,
        object checkboxHtmlAttributes = null)
    {
        var selectList = GetCountries();

        return html.CheckBoxList(
            name,
            selectList,
            selectedIds,
            labelHtmlAttributes: labelHtmlAttributes,
            checkboxHtmlAttributes: checkboxHtmlAttributes);
    }

    public IHtmlContent CountryCheckBoxListFor(
        Expression<Func<TModel, IEnumerable<string>>> expression,
        object labelHtmlAttributes = null,
        object checkboxHtmlAttributes = null)
    {
        var selectList = GetCountries();

        return html.CheckBoxListFor(
            expression,
            selectList,
            labelHtmlAttributes: labelHtmlAttributes,
            checkboxHtmlAttributes: checkboxHtmlAttributes);
    }

    #endregion Countries

    private static IEnumerable<SelectListItem> GetContinents(int? selectedValue = null, string emptyText = null)
    {
        var service = EngineContext.Current.Resolve<IRegionService>();
        var workContext = EngineContext.Current.Resolve<IWorkContext>();

        var records = service.GetContinents(workContext.CurrentTenant.Id, workContext.CurrentCultureCode)
            .OrderBy(x => x.Order == null)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Name);

        return records.ToSelectList(
            value => value.Id,
            text => text.Name,
            selectedValue,
            emptyText);
    }

    private static IEnumerable<SelectListItem> GetCountries(int? selectedValue = null, string emptyText = null)
    {
        var service = EngineContext.Current.Resolve<IRegionService>();
        var workContext = EngineContext.Current.Resolve<IWorkContext>();

        var records = service.GetCountries(workContext.CurrentTenant.Id, workContext.CurrentCultureCode)
            .OrderBy(x => x.Order == null)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Name);

        return records.ToSelectList(
            value => value.Id,
            text => text.Name,
            selectedValue,
            emptyText);
    }

    #endregion Regions
}