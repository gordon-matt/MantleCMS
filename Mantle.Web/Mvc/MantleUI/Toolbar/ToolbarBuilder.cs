using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Web.Mvc.MantleUI
{
    public class ToolbarBuilder<TModel> : BuilderBase<TModel, Toolbar>
    {
        internal ToolbarBuilder(IHtmlHelper<TModel> htmlHelper, Toolbar toolbar)
            : base(htmlHelper, toolbar)
        {
        }

        public ButtonGroup BeginButtonGroup()
        {
            return new ButtonGroup(base.element.Provider, base.textWriter);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}