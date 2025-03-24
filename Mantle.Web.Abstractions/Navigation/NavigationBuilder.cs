namespace Mantle.Web.Navigation;

public class NavigationBuilder
{
    private IEnumerable<MenuItem> Contained { get; set; }

    public NavigationBuilder Add(
        string caption,
        string position,
        Action<NavigationItemBuilder> itemBuilder,
        string cssClass = null)
    {
        var childBuilder = new NavigationItemBuilder();

        childBuilder.Caption(caption);
        childBuilder.Position(position);
        itemBuilder(childBuilder);
        Contained = (Contained ?? Enumerable.Empty<MenuItem>()).Concat(childBuilder.Build());

        if (!string.IsNullOrEmpty(cssClass))
        {
            childBuilder.CssClass(cssClass);
        }

        return this;
    }

    public NavigationBuilder Add(MenuItem menuItem)
    {
        Contained = (Contained ?? Enumerable.Empty<MenuItem>()).Concat(new[] { menuItem });
        return this;
    }

    public NavigationBuilder Add<TParent>(
        string caption,
        string position,
        Action<NavigationItemBuilder, TParent> itemBuilder,
        TParent parent,
        string cssClass = null)
    {
        var childBuilder = new NavigationItemBuilder();
        childBuilder.Caption(caption);
        childBuilder.Position(position);
        itemBuilder(childBuilder, parent);
        Contained = (Contained ?? Enumerable.Empty<MenuItem>()).Concat(childBuilder.Build());

        if (!string.IsNullOrEmpty(cssClass))
        {
            childBuilder.CssClass(cssClass);
        }

        return this;
    }

    public NavigationBuilder Add(
        string caption,
        Action<NavigationItemBuilder> itemBuilder,
        string cssClass = null) => Add(caption, null, itemBuilder, cssClass);

    public NavigationBuilder Add(Action<NavigationItemBuilder> itemBuilder, string cssClass = null) => Add(null, null, itemBuilder, cssClass);

    public NavigationBuilder Add(string caption, string cssClass = null) => Add(caption, null, x => { }, cssClass);

    public virtual IEnumerable<MenuItem> Build() => (Contained ?? Enumerable.Empty<MenuItem>()).ToList();
}