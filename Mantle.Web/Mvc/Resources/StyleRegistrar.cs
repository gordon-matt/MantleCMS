using System;
using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Mantle.Web.Mvc.Resources
{
    public class StyleRegistrar : ResourceRegistrar
    {
        private readonly IUrlHelper urlHelper;

        public StyleRegistrar(IWebWorkContext workContext)
            : base(workContext)
        {
            var urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>();
            var actionContextAccessor = EngineContext.Current.Resolve<IActionContextAccessor>();
            urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        protected override string BundleBasePath => "/css/bundles/";

        protected override string VirtualBasePath => "/css"; // TODO: Make configurable?

        protected override ResourceLocation DefaultLocation => ResourceLocation.Head;

        public override void IncludeInline(string code, ResourceLocation? location = null, bool ignoreIfExists = false)
        {
            throw new NotSupportedException();
        }

        protected override string BuildInlineResources(IEnumerable<string> resources)
        {
            return string.Format("<style type=\"text/css\">{0}</style>", string.Join(Environment.NewLine, resources));
        }

        protected override string BuildResource(ResourceEntry resource)
        {
            var builder = new FluentTagBuilder("link", TagRenderMode.SelfClosing)
                .MergeAttribute("type", "text/css")
                .MergeAttribute("rel", "stylesheet")
                .MergeAttribute("href", urlHelper.Content(resource.Path))
                .MergeAttributes(resource.HtmlAttributes);

            return builder.ToString();
        }
    }
}