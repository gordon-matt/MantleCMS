namespace Mantle.Web.Mvc.MantleUI;

public class Panel : HtmlElement
{
    public string Id { get; private set; }

    public State State { get; private set; }

    public Panel(string id = null, State state = State.Primary, object htmlAttributes = null)
        : base(htmlAttributes)
    {
        if (id == null)
        {
            id = "panel-" + Guid.NewGuid();
        }
        this.Id = id;
        this.State = state;
        EnsureHtmlAttribute("id", this.Id);
    }

    protected internal override void StartTag(TextWriter textWriter)
    {
        Provider.PanelProvider.BeginPanel(this, textWriter);
    }

    protected internal override void EndTag(TextWriter textWriter)
    {
        Provider.PanelProvider.EndPanel(this, textWriter);
    }
}