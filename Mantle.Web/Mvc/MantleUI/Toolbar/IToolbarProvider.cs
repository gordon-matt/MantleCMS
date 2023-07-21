namespace Mantle.Web.Mvc.MantleUI
{
    public interface IToolbarProvider
    {
        void BeginToolbar(Toolbar toolbar, TextWriter writer);

        void BeginButtonGroup(TextWriter writer);

        void EndButtonGroup(TextWriter writer);

        void EndToolbar(Toolbar toolbar, TextWriter writer);

        void AddButton(TextWriter writer, string text, State state, string onClick = null, object htmlAttributes = null);
    }
}