using System;
using Extenso.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mantle.Web.Mvc.MantleUI.Providers
{
    public class Bootstrap3UIProvider : BaseUIProvider
    {
        private readonly IUrlHelper urlHelper;

        public Bootstrap3UIProvider()
        {
            urlHelper = MvcHelpers.UrlHelper;
        }

        private IAccordionProvider accordionProvider;
        private IModalProvider modalProvider;
        private IPanelProvider panelProvider;
        private ITabsProvider tabsProvider;
        private IThumbnailProvider thumbnailProvider;
        private IToolbarProvider toolbarProvider;

        #region IMantleUIProvider Members

        #region General

        public override IHtmlContent ActionLink(IHtmlHelper html, string text, State state, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
        {
            string stateCss = GetButtonCssClass(state);

            var builder = new FluentTagBuilder("a")
                .MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
                .MergeAttribute("href", urlHelper.Action(actionName, controllerName, routeValues))
                .AddCssClass(stateCss)
                .SetInnerHtml(text);

            return new HtmlString(builder.ToString());
        }

        public override IHtmlContent Badge(string text, object htmlAttributes = null)
        {
            var builder = new FluentTagBuilder("span")
                .MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
                .AddCssClass("badge")
                .SetInnerHtml(text);

            return new HtmlString(builder.ToString());
        }

        public override IHtmlContent Button(string text, State state, string onClick = null, object htmlAttributes = null)
        {
            string stateCss = GetButtonCssClass(state);

            var builder = new FluentTagBuilder("button")
                .MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
                .MergeAttribute("type", "button")
                .AddCssClass(stateCss)
                .SetInnerHtml(text);

            if (!string.IsNullOrEmpty(onClick))
            {
                builder.MergeAttribute("onclick", onClick);
            }

            return new HtmlString(builder.ToString());
        }

        public override IHtmlContent InlineLabel(string text, State state, object htmlAttributes = null)
        {
            string stateCss = GetButtonCssClass(state);

            var builder = new FluentTagBuilder("span")
                .MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
                .AddCssClass(stateCss)
                .SetInnerHtml(text);

            return new HtmlString(builder.ToString());
        }

        public override IHtmlContent Quote(string text, string author, string titleOfWork, object htmlAttributes = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var builder = new FluentTagBuilder("blockquote")
                .MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
                .StartTag("p")
                    .SetInnerHtml(text)
                .EndTag()
                .StartTag("footer")
                    .AppendContent(author + ", ")
                    .StartTag("cite")
                        .MergeAttribute("title", titleOfWork)
                        .SetInnerHtml(titleOfWork)
                    .EndTag()
                .EndTag();

            return new HtmlString(builder.ToString());
        }

        public override IHtmlContent SubmitButton(string text, State state, object htmlAttributes = null)
        {
            string stateCss = GetButtonCssClass(state);

            var builder = new FluentTagBuilder("button")
                .MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
                .MergeAttribute("type", "submit")
                .AddCssClass(stateCss)
                .SetInnerHtml(text);

            return new HtmlString(builder.ToString());
        }

        public override IHtmlContent TextBoxWithAddOns(IHtmlHelper html, string name, object value, string prependValue, string appendValue, object htmlAttributes = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var builder = new FluentTagBuilder("div")
                .AddCssClass("input-group");

            if (!string.IsNullOrEmpty(prependValue))
            {
                builder = builder
                    .StartTag("div")
                        .AddCssClass("input-group-addon")
                        .SetInnerHtml(prependValue)
                    .EndTag();
            }

            builder = builder.AppendContent(html.TextBox(name, value, htmlAttributes).ToString());

            if (!string.IsNullOrEmpty(appendValue))
            {
                builder = builder
                .StartTag("div")
                    .AddCssClass("input-group-addon")
                    .SetInnerHtml(appendValue)
                .EndTag();
            }

            return new HtmlString(builder.ToString());
        }

        #endregion General

        #region Special

        public override IAccordionProvider AccordionProvider
        {
            get { return accordionProvider ?? (accordionProvider = new Bootstrap3AccordionProvider()); }
        }

        public override IModalProvider ModalProvider
        {
            get { return modalProvider ?? (modalProvider = new Bootstrap3ModalProvider()); }
        }

        public override IPanelProvider PanelProvider
        {
            get { return panelProvider ?? (panelProvider = new Bootstrap3PanelProvider()); }
        }

        public override ITabsProvider TabsProvider
        {
            get { return tabsProvider ?? (tabsProvider = new Bootstrap3TabsProvider()); }
        }

        public override IThumbnailProvider ThumbnailProvider
        {
            get { return thumbnailProvider ?? (thumbnailProvider = new Bootstrap3ThumbnailProvider()); }
        }

        public override IToolbarProvider ToolbarProvider
        {
            get { return toolbarProvider ?? (toolbarProvider = new Bootstrap3ToolbarProvider(this)); }
        }

        #endregion Special

        #endregion IMantleUIProvider Members

        protected override string GetButtonCssClass(State state)
        {
            switch (state)
            {
                case State.Important: return "btn btn-danger";
                case State.Default: return "btn btn-default";
                case State.Info: return "btn btn-info";
                case State.Inverse: return "btn btn-inverse";
                case State.Primary: return "btn btn-primary";
                case State.Success: return "btn btn-success";
                case State.Warning: return "btn btn-warning";
                default: return "btn btn-default";
            }
        }

        protected override string GetLabelCssClass(State state)
        {
            switch (state)
            {
                case State.Important: return "label label-danger";
                case State.Default: return "label label-default";
                case State.Info: return "label label-info";
                case State.Inverse: return "label label-inverse";
                case State.Primary: return "label label-primary";
                case State.Success: return "label label-success";
                case State.Warning: return "label label-warning";
                default: return "label label-default";
            }
        }
    }
}