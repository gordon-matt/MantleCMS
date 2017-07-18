using System;
using System.IO;

namespace Mantle.Web.Mvc.MantleUI
{
    public class Toolbar : HtmlElement
    {
        public string Id { get; private set; }

        public Toolbar(string id = null, object htmlAttributes = null)
            : base(htmlAttributes)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "toolbar-" + Guid.NewGuid();
            }

            this.Id = id;
            EnsureHtmlAttribute("id", this.Id);
        }

        protected internal override void StartTag(TextWriter textWriter)
        {
            Provider.ToolbarProvider.BeginToolbar(this, textWriter);
        }

        protected internal override void EndTag(TextWriter textWriter)
        {
            Provider.ToolbarProvider.EndToolbar(this, textWriter);
        }
    }
}