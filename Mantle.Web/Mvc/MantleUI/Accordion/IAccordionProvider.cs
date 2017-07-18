using System.IO;

namespace Mantle.Web.Mvc.MantleUI
{
    public interface IAccordionProvider
    {
        void BeginAccordion(Accordion accordion, TextWriter writer);

        void BeginAccordionPanel(TextWriter writer, string title, string panelId, string parentAccordionId, bool expanded);

        void EndAccordion(Accordion accordion, TextWriter writer);

        void EndAccordionPanel(TextWriter writer);
    }
}