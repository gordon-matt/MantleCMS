using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Web.Mvc.MantleUI.Providers
{
    public interface IKoreUIProvider
    {
        IHtmlContent RenderScripts();

        #region General

        IHtmlContent ActionLink(IHtmlHelper html, string text, State state, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null);

        IHtmlContent Badge(string text, object htmlAttributes = null);

        IHtmlContent Button(string text, State state, string onClick = null, object htmlAttributes = null);

        IHtmlContent InlineLabel(string text, State state, object htmlAttributes = null);

        IHtmlContent Quote(string text, string author, string titleOfWork, object htmlAttributes = null);

        IHtmlContent SubmitButton(string text, State state, object htmlAttributes = null);

        IHtmlContent TextBoxWithAddOns(IHtmlHelper html, string name, object value, string prependValue, string appendValue, object htmlAttributes = null);

        #endregion General

        #region Special

        IAccordionProvider AccordionProvider { get; }

        IModalProvider ModalProvider { get; }

        IPanelProvider PanelProvider { get; }

        ITabsProvider TabsProvider { get; }

        IThumbnailProvider ThumbnailProvider { get; }

        IToolbarProvider ToolbarProvider { get; }

        #endregion Special
    }
}