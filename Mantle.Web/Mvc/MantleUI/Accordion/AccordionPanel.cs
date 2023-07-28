namespace Mantle.Web.Mvc.MantleUI;

public class AccordionPanel : IDisposable
{
    private readonly TextWriter textWriter;
    private readonly IMantleUIProvider provider;

    internal AccordionPanel(IMantleUIProvider provider, TextWriter writer, string title, string panelId, string parentAccordionId, bool expanded = false)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title));
        }

        this.provider = provider;
        this.textWriter = writer;

        provider.AccordionProvider.BeginAccordionPanel(this.textWriter, title, panelId, parentAccordionId, expanded);
    }

    public void Dispose()
    {
        provider.AccordionProvider.EndAccordionPanel(this.textWriter);
    }
}