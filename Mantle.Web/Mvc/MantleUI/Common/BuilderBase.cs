using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Web.Mvc.MantleUI
{
    public abstract class BuilderBase<TModel, T> : IDisposable where T : HtmlElement
    {
        // Fields
        protected readonly T element;

        protected readonly TextWriter textWriter;
        protected readonly IHtmlHelper<TModel> htmlHelper;

        // Methods
        public BuilderBase(IHtmlHelper<TModel> htmlHelper, T element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            this.element = element;
            this.textWriter = htmlHelper.ViewContext.Writer;
            this.element.StartTag(textWriter);
            this.htmlHelper = htmlHelper;
        }

        public virtual void Dispose()
        {
            this.element.EndTag(textWriter);
        }
    }
}