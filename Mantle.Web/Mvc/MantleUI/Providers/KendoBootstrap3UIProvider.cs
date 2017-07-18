namespace Mantle.Web.Mvc.MantleUI.Providers
{
    public class KendoBootstrap3UIProvider : Bootstrap3UIProvider
    {
        private IAccordionProvider accordionProvider;
        private IModalProvider modalProvider;
        private ITabsProvider tabsProvider;
        //private IToolbarProvider toolbarProvider;

        #region IKoreUIProvider Members

        public override IAccordionProvider AccordionProvider
        {
            get { return accordionProvider ?? (accordionProvider = new KendoUIAccordionProvider(this)); }
        }

        public override IModalProvider ModalProvider
        {
            get { return modalProvider ?? (modalProvider = new KendoUIModalProvider(this)); }
        }

        public override ITabsProvider TabsProvider
        {
            get { return tabsProvider ?? (tabsProvider = new KendoUITabsProvider(this)); }
        }

        //public override IToolbarProvider ToolbarProvider
        //{
        //    get { return toolbarProvider ?? (toolbarProvider = new KendoUIToolbarProvider()); }
        //}

        #endregion IKoreUIProvider Members

        protected override string GetButtonCssClass(State state)
        {
            switch (state)
            {
                case State.Primary: return "k-primary k-button";
                default: return "k-button";
            }
        }
    }
}