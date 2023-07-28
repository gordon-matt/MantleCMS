namespace Mantle.Web.Mvc.MantleUI;

public enum PanelSectionType : byte
{
    Heading,
    Body,
    Footer
}

public class PanelSection : IDisposable
{
    private readonly TextWriter textWriter;
    private readonly IMantleUIProvider provider;

    public PanelSectionType SectionType { get; private set; }

    internal PanelSection(IMantleUIProvider provider, PanelSectionType sectionType, TextWriter writer, string title = null)
    {
        this.provider = provider;
        this.SectionType = sectionType;
        this.textWriter = writer;
        provider.PanelProvider.BeginPanelSection(this.SectionType, this.textWriter, title);
    }

    public void Dispose()
    {
        provider.PanelProvider.EndPanelSection(this.SectionType, this.textWriter);
    }
}