using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Mantle.Web.Mvc.MantleUI
{
    public class ThumbnailBuilder<TModel> : BuilderBase<TModel, Thumbnail>
    {
        private readonly IUrlHelper urlHelper;

        internal ThumbnailBuilder(IHtmlHelper<TModel> htmlHelper, Thumbnail thumbnail)
            : base(htmlHelper, thumbnail)
        {
            urlHelper = MvcHelpers.UrlHelper;
            base.element.Provider.ThumbnailProvider.WriteThumbnailImage(this.element, urlHelper, base.textWriter);
        }

        public ThumbnailCaptionPanel BeginCaptionPanel()
        {
            return new ThumbnailCaptionPanel(base.element.Provider, base.textWriter);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}