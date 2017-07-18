using System;
using System.IO;
using Mantle.Web.Mvc.MantleUI.Providers;

namespace Mantle.Web.Mvc.MantleUI
{
    public class ThumbnailCaptionPanel : IDisposable
    {
        private readonly IKoreUIProvider provider;
        private readonly TextWriter textWriter;

        internal ThumbnailCaptionPanel(IKoreUIProvider provider, TextWriter writer)
        {
            this.provider = provider;
            this.textWriter = writer;
            provider.ThumbnailProvider.BeginCaptionPanel(this.textWriter);
        }

        public void Dispose()
        {
            provider.ThumbnailProvider.EndCaptionPanel(this.textWriter);
        }
    }
}