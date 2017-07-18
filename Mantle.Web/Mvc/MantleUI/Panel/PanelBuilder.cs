using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Web.Mvc.MantleUI
{
    public class PanelBuilder<TModel> : BuilderBase<TModel, Panel>
    {
        internal PanelBuilder(IHtmlHelper<TModel> htmlHelper, Panel panel)
            : base(htmlHelper, panel)
        {
        }

        public PanelSection BeginHeader(string title)
        {
            return new PanelSection(base.element.Provider, PanelSectionType.Heading, base.textWriter, title);
        }

        public PanelSection BeginBody()
        {
            return new PanelSection(base.element.Provider, PanelSectionType.Body, base.textWriter);
        }

        public PanelSection BeginFooter()
        {
            return new PanelSection(base.element.Provider, PanelSectionType.Footer, base.textWriter);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}