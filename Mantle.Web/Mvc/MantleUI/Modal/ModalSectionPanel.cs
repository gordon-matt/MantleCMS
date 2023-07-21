using Mantle.Web.Mvc.MantleUI.Providers;
using Microsoft.AspNetCore.Html;

namespace Mantle.Web.Mvc.MantleUI
{
    public enum ModalSection
    {
        Header,
        Body,
        Footer
    }

    public class ModalSectionPanel : IDisposable
    {
        private readonly TextWriter textWriter;
        private readonly IMantleUIProvider provider;

        public ModalSection Section { get; private set; }

        internal ModalSectionPanel(IMantleUIProvider provider, ModalSection section, TextWriter writer, string title = null)
        {
            this.provider = provider;
            this.Section = section;
            this.textWriter = writer;
            provider.ModalProvider.BeginModalSectionPanel(this.Section, this.textWriter, title);
        }

        public IHtmlContent ModalCloseButton(string modalId, string text, object htmlAttributes = null)
        {
            return provider.ModalProvider.ModalCloseButton(modalId, text, htmlAttributes);
        }

        public void Dispose()
        {
            provider.ModalProvider.EndModalSectionPanel(this.Section, this.textWriter);
        }
    }
}