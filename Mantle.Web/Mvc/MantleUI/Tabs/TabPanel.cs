using System;
using System.IO;
using Mantle.Web.Mvc.MantleUI.Providers;

namespace Mantle.Web.Mvc.MantleUI
{
    public class TabPanel : IDisposable
    {
        public string Id { get; private set; }

        public bool IsActive { get; private set; }

        private readonly TextWriter textWriter;
        private readonly IKoreUIProvider provider;

        internal TabPanel(IKoreUIProvider provider, TextWriter writer, string id, bool isActive = false)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            this.provider = provider;
            this.Id = id;
            this.IsActive = isActive;
            this.textWriter = writer;
            provider.TabsProvider.BeginTabPanel(this, this.textWriter);
        }

        public void Dispose()
        {
            provider.TabsProvider.EndTabPanel(this.textWriter);
        }
    }
}