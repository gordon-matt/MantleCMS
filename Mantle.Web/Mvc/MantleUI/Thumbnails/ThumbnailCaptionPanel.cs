namespace Mantle.Web.Mvc.MantleUI;

public class ThumbnailCaptionPanel : IDisposable
{
    private readonly IMantleUIProvider provider;
    private readonly TextWriter textWriter;

    internal ThumbnailCaptionPanel(IMantleUIProvider provider, TextWriter writer)
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