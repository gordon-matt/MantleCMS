using System;
using System.IO;
using Mantle.Web.Mvc.MantleUI.Providers;

namespace Mantle.Web.Mvc.MantleUI
{
    public class ButtonGroup : IDisposable
    {
        private readonly TextWriter textWriter;
        private readonly IKoreUIProvider provider;

        internal ButtonGroup(IKoreUIProvider provider, TextWriter writer)
        {
            this.provider = provider;
            this.textWriter = writer;
            provider.ToolbarProvider.BeginButtonGroup(this.textWriter);
        }

        public void Button(string text, State state, string onClick = null, object htmlAttributes = null)
        {
            provider.ToolbarProvider.AddButton(this.textWriter, text, state, onClick, htmlAttributes);
        }

        public void Dispose()
        {
            provider.ToolbarProvider.EndButtonGroup(this.textWriter);
        }
    }
}