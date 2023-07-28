namespace Mantle.Web.Mvc.MantleUI;

public class Bootstrap3PanelProvider : IPanelProvider
{
    #region IPanelProvider Members

    public void BeginPanel(Panel panel, TextWriter writer)
    {
        switch (panel.State)
        {
            case State.Default: panel.EnsureClass("panel panel-default"); break;
            case State.Important: panel.EnsureClass("panel panel-danger"); break;
            case State.Info: panel.EnsureClass("panel panel-info"); break;
            case State.Inverse: panel.EnsureClass("panel panel-inverse"); break;
            case State.Primary: panel.EnsureClass("panel panel-primary"); break;
            case State.Success: panel.EnsureClass("panel panel-success"); break;
            case State.Warning: panel.EnsureClass("panel panel-warning"); break;
        }

        var builder = new TagBuilder("div");
        builder.TagRenderMode = TagRenderMode.StartTag;
        builder.MergeAttributes(panel.HtmlAttributes);
        string tag = builder.Build();

        writer.Write(tag);
    }

    public void BeginPanelSection(PanelSectionType sectionType, TextWriter writer, string title = null)
    {
        switch (sectionType)
        {
            case PanelSectionType.Heading:
                {
                    writer.Write(string.Format(
@"<div class=""panel-heading"">
    <h3 class=""panel-title"">{0}</h3>", title));
                }
                break;

            case PanelSectionType.Body:
                writer.Write(@"<div class=""panel-body"">");
                break;

            case PanelSectionType.Footer:
                writer.Write(@"<div class=""panel-footer"">");
                break;
        }
    }

    public void EndPanel(Panel panel, TextWriter writer)
    {
        writer.Write("</div>");
    }

    public void EndPanelSection(PanelSectionType sectionType, TextWriter writer)
    {
        writer.Write("</div>");
    }

    #endregion IPanelProvider Members
}