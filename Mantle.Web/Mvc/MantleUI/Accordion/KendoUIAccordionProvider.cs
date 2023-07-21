using Extenso.AspNetCore.Mvc.Rendering;
using Mantle.Web.Mvc.MantleUI.Providers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Web.Mvc.MantleUI
{
    public class KendoUIAccordionProvider : IAccordionProvider
    {
        private readonly KendoBootstrap3UIProvider uiProvider;

        public KendoUIAccordionProvider(KendoBootstrap3UIProvider uiProvider)
        {
            this.uiProvider = uiProvider;
        }

        #region IAccordionProvider Members

        public string AccordionTag
        {
            get { return "ul"; }
        }

        public void BeginAccordion(Accordion accordion, TextWriter writer)
        {
            uiProvider.Scripts.Add(string.Format(
@"$('#{0}').kendoPanelBar({{
    expandMode: 'single'
}});", accordion.Id));

            var builder = new TagBuilder("ul");
            builder.TagRenderMode = TagRenderMode.StartTag;
            builder.MergeAttributes<string, object>(accordion.HtmlAttributes);
            string tag = builder.Build();

            writer.Write(tag);
        }

        public void BeginAccordionPanel(TextWriter writer, string title, string panelId, string parentAccordionId, bool expanded)
        {
            var li = new FluentTagBuilder("li", TagRenderMode.StartTag);

            if (expanded)
            {
                li.AddCssClass("k-state-active");
            }

            writer.Write(li.ToString());
            writer.Write(title);
            writer.Write(@"<div class=""k-content"">");
        }

        public void EndAccordion(Accordion accordion, TextWriter writer)
        {
            writer.Write("</ul>");
        }

        public void EndAccordionPanel(TextWriter writer)
        {
            writer.Write("</div></li>");
        }

        #endregion IAccordionProvider Members
    }
}