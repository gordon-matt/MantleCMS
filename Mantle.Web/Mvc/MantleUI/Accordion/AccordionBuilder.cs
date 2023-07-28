namespace Mantle.Web.Mvc.MantleUI;

public class AccordionBuilder<TModel> : BuilderBase<TModel, Accordion>
{
    internal AccordionBuilder(IHtmlHelper<TModel> htmlHelper, Accordion accordion)
        : base(htmlHelper, accordion)
    {
    }

    public AccordionPanel BeginPanel(string title, string id, bool expanded = false)
    {
        return new AccordionPanel(base.element.Provider, base.textWriter, title, id, base.element.Id, expanded);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}