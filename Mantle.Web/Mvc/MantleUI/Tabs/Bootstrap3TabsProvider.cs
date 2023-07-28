namespace Mantle.Web.Mvc.MantleUI;

public class Bootstrap3TabsProvider : ITabsProvider
{
    #region ITabsProvider Members

    public void BeginTabs(Tabs tabs, TextWriter writer)
    {
        tabs.EnsureHtmlAttribute("role", "tabpanel");

        switch (tabs.Position)
        {
            case TabPosition.Left: tabs.EnsureClass("tabbable tabs-left"); break;
            case TabPosition.Right: tabs.EnsureClass("tabbable tabs-right"); break;
                //case TabPosition.Bottom: tabs.EnsureClass("tabbable tabs-bottom"); break;
        }

        var builder = new TagBuilder("div");
        builder.TagRenderMode = TagRenderMode.StartTag;
        builder.MergeAttributes<string, object>(tabs.HtmlAttributes);
        string tag = builder.Build();

        writer.Write(tag);
    }

    public void BeginTabsHeader(TextWriter writer)
    {
        writer.Write(@"<ul class=""nav nav-tabs"" role=""tablist"">");
    }

    public void BeginTabContent(TextWriter writer)
    {
        writer.Write(@"<div class=""tab-content"">");
    }

    public void BeginTabPanel(TabPanel panel, TextWriter writer)
    {
        var builder = new TagBuilder("div");
        builder.TagRenderMode = TagRenderMode.StartTag;
        builder.MergeAttribute("id", panel.Id);
        builder.MergeAttribute("role", "tabpanel");
        builder.AddCssClass("tab-pane");

        if (panel.IsActive)
        {
            builder.AddCssClass("active");
        }

        writer.Write(builder.Build());
    }

    public void EndTabPanel(TextWriter writer)
    {
        writer.Write("</div>");
    }

    public void EndTabsHeader(TextWriter writer)
    {
        writer.Write("</ul>");
    }

    public void EndTabs(Tabs tabs, TextWriter writer)
    {
        writer.Write("</div></div>");
    }

    public void WriteTab(TextWriter writer, string label, string tabId, bool isActive)
    {
        if (isActive)
        {
            writer.Write(string.Format(
                @"<li role=""presentation"" class=""active""><a href=""#{0}"" aria-controls=""{0}"" role=""tab"" data-toggle=""tab"">{1}</a></li>",
                tabId,
                label));
        }
        else
        {
            writer.Write(string.Format(
                @"<li role=""presentation""><a href=""#{0}"" aria-controls=""{0}"" role=""tab"" data-toggle=""tab"">{1}</a></li>",
                tabId,
                label));
        }
    }

    #endregion ITabsProvider Members
}