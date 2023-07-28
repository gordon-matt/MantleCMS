//No license, but thanks to Matteo Tontini
//http://ilmatte.wordpress.com/2010/11/16/asp-net-mvc-panel-htmlhelper-extension-methods-with-using-syntax/


using System.Diagnostics.CodeAnalysis;

namespace Mantle.Web.Mvc.Controls;
/// <summary>
/// Extension methods for <see cref="HtmlHelper"/>
/// to add rendering capability for panels.
/// </summary>
public static class PanelExtensions
{
    #region Constants

    private const string TitleCssClass = "panelTitle";
    private const string PanelCssClass = "panel";

    #endregion Constants

    #region BeginPanel

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title)
    {
        return BeginPanel(helper, title, new RouteValueDictionary(), new RouteValueDictionary());
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title, object htmlAttributes)
    {
        return BeginPanel(helper, title, new RouteValueDictionary(), new RouteValueDictionary(htmlAttributes));
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title, IDictionary<string, object> htmlAttributes)
    {
        return BeginPanel(helper, title, new RouteValueDictionary(), htmlAttributes);
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <param name="titleHtmlAttributes">Attributes to apply to the title panel.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title, object titleHtmlAttributes, object htmlAttributes)
    {
        return BeginPanel(helper, title, TitleCssClass, PanelCssClass, new RouteValueDictionary(titleHtmlAttributes), new RouteValueDictionary(htmlAttributes));
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <param name="titleHtmlAttributes">Attributes to apply to the title panel.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title, IDictionary<string, object> titleHtmlAttributes, IDictionary<string, object> htmlAttributes)
    {
        return BeginPanel(helper, title, TitleCssClass, PanelCssClass, titleHtmlAttributes, htmlAttributes);
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <param name="panelCssClass">The css class rule to apply
    /// to the panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title, string panelCssClass)
    {
        return BeginPanel(helper, title, TitleCssClass, panelCssClass);
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>./// <summary>
    /// Renders <div class="{title css class}">{title}</div>
    /// <div class="{css class}">.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <param name="titlePanelCssClass">The css class rule to apply
    /// to the title bar.</param>
    /// <param name="panelCssClass">The css class rule to apply
    /// to the panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title, string titleCssClass, string panelCssClass)
    {
        return BeginPanel(helper, title, titleCssClass, panelCssClass, new RouteValueDictionary(), new RouteValueDictionary());
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <param name="titlePanelCssClass">The css class rule to apply
    /// to the title bar.</param>
    /// <param name="panelCssClass">The css class rule to apply
    /// to the panel.</param>
    /// <param name="titleHtmlAttributes">Attributes to apply to the title panel.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title, string titleCssClass, string panelCssClass, object titleHtmlAttributes, object panelHtmlAttributes)
    {
        return BeginPanel(helper, title, titleCssClass, panelCssClass, new RouteValueDictionary(titleHtmlAttributes), new RouteValueDictionary(panelHtmlAttributes));
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="title">The title to render in the panel title bar.</param>
    /// <param name="titlePanelCssClass">The css class rule to apply
    /// to the title bar.</param>
    /// <param name="panelCssClass">The css class rule to apply
    /// to the panel.</param>
    /// <param name="titleHtmlAttributes">Attributes to apply to the title panel.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    public static MvcPanel BeginPanel(this HtmlHelper helper, string title, string titleCssClass, string panelCssClass, IDictionary<string, object> titleHtmlAttributes, IDictionary<string, object> panelHtmlAttributes)
    {
        // title panel
        var titleTagBuilder = new TagBuilder("div");
        titleTagBuilder.TagRenderMode = TagRenderMode.Normal;
        titleTagBuilder.MergeAttributes(titleHtmlAttributes);
        titleTagBuilder.AddCssClass(titleCssClass);
        titleTagBuilder.InnerHtml.Append(title);

        // content panel
        var tagBuilder = new TagBuilder("div");
        tagBuilder.TagRenderMode = TagRenderMode.StartTag;
        tagBuilder.MergeAttributes(panelHtmlAttributes);
        tagBuilder.AddCssClass(panelCssClass);

        helper.ViewContext.Writer.Write(titleTagBuilder.Build());
        helper.ViewContext.Writer.Write(tagBuilder.Build());
        return new MvcPanel(helper.ViewContext.Writer);
    }

    #endregion BeginPanel

    #region BeginPanelFor

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string);
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string, new RouteValueDictionary(), new RouteValueDictionary(htmlAttributes));
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string, new RouteValueDictionary(), htmlAttributes);
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <param name="titleHtmlAttributes">Attributes to apply to the title panel.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object titleHtmlAttributes, object htmlAttributes)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string, TitleCssClass, PanelCssClass, new RouteValueDictionary(titleHtmlAttributes), new RouteValueDictionary(htmlAttributes));
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <param name="titleHtmlAttributes">Attributes to apply to the title panel.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> titleHtmlAttributes, IDictionary<string, object> htmlAttributes)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string, TitleCssClass, PanelCssClass, titleHtmlAttributes, htmlAttributes);
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <param name="panelCssClass">The css class rule to apply
    /// to the panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string panelCssClass)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string, TitleCssClass, panelCssClass);
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <param name="titlePanelCssClass">The css class rule to apply
    /// to the title bar.</param>
    /// <param name="panelCssClass">The css class rule to apply
    /// to the panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string titleCssClass, string panelCssClass)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string, titleCssClass, panelCssClass, new RouteValueDictionary(), new RouteValueDictionary());
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <param name="titlePanelCssClass">The css class rule to apply
    /// to the title bar.</param>
    /// <param name="panelCssClass">The css class rule to apply
    /// to the panel.</param>
    /// <param name="titleHtmlAttributes">Attributes to apply to the title panel.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string titleCssClass, string panelCssClass, object titleHtmlAttributes, object panelHtmlAttributes)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string, titleCssClass, panelCssClass, new RouteValueDictionary(titleHtmlAttributes), new RouteValueDictionary(panelHtmlAttributes));
    }

    /// <summary>
    /// Renders <![CDATA[<div class="{title css class}">{title}</div>
    /// <div class="{css class}">]]>.
    /// Exactly the same as the overload accepting a string
    /// representing the title, with the difference that here
    /// the title is retrieved from the <see cref="Expression"/>.
    /// The panel is rendered as a <![CDATA[<div>]]> for the title
    /// and a <![CDATA[<div>]]> for the content wrapper.
    /// This method renders the title panel and the opening tag
    /// of the content panel, leaving space for content.
    /// Both the title bar and the panel will be styled
    /// with default css class rules.
    /// The method sets:
    /// a css class rule named: 'panelTitle' on the title div
    /// and a css class rule named: 'panel' on the content div.
    /// The desired style must be provided to render the menu as desired.
    /// </summary>
    /// <typeparam name="TModel">The type of the model that
    /// will be passed to the expression.</typeparam>
    /// <typeparam name="TValue">The type of the value returned
    /// by the expression.</typeparam>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    /// <param name="expression">The expression accepting a model instance
    /// and returning the value to be used as title.</param>
    /// <param name="titlePanelCssClass">The css class rule to apply
    /// to the title bar.</param>
    /// <param name="panelCssClass">The css class rule to apply
    /// to the panel.</param>
    /// <param name="titleHtmlAttributes">Attributes to apply to the title panel.</param>
    /// <param name="htmlAttributes">Attributes to apply to the content panel.</param>
    /// <returns>The beginning of a panel (a div for the title and
    /// the opening tag of the div for the content).</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcPanel BeginPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string titleCssClass, string panelCssClass, IDictionary<string, object> titleHtmlAttributes, IDictionary<string, object> panelHtmlAttributes)
    {
        // TODO: Test
        var modelExpressionProvider = EngineContext.Current.Resolve<ModelExpressionProvider>();
        var modelExpression = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), modelExpression.Model);
        return BeginPanel(html, modelExplorer.Model as string, titleCssClass, panelCssClass, titleHtmlAttributes, panelHtmlAttributes);
    }

    #endregion BeginPanelFor

    /// <summary>
    /// Renders the closing tag of the panel: a
    /// closing <![CDATA[</div>]]> tag.
    /// </summary>
    /// <param name="helper">The <see cref="HtmlHelper"/> instance
    /// that this method extends.</param>
    public static void EndPanel(this HtmlHelper helper)
    {
        helper.ViewContext.Writer.Write("</div>");
    }
}