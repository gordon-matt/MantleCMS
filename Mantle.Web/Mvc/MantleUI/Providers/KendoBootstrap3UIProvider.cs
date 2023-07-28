namespace Mantle.Web.Mvc.MantleUI.Providers;

public class KendoBootstrap3UIProvider : Bootstrap3UIProvider
{
    private IAccordionProvider accordionProvider;
    private IModalProvider modalProvider;
    private ITabsProvider tabsProvider;
    //private IToolbarProvider toolbarProvider;

    #region IMantleUIProvider Members

    public override IAccordionProvider AccordionProvider
    {
        get { return accordionProvider ??= new KendoUIAccordionProvider(this); }
    }

    public override IModalProvider ModalProvider
    {
        get { return modalProvider ??= new KendoUIModalProvider(this); }
    }

    public override ITabsProvider TabsProvider
    {
        get { return tabsProvider ??= new KendoUITabsProvider(this); }
    }

    //public override IToolbarProvider ToolbarProvider
    //{
    //    get { return toolbarProvider ?? (toolbarProvider = new KendoUIToolbarProvider()); }
    //}

    #endregion IMantleUIProvider Members

    protected override string GetButtonCssClass(State state)
    {
        return state switch
        {
            State.Primary => "k-primary k-button",
            _ => "k-button",
        };
    }
}