namespace Mantle.Web.Messaging;

public interface IMessageTemplateEditor
{
    string Name { get; }

    /// <summary>
    /// Use {0} for template ID and {1} for culture code
    /// </summary>
    string UrlFormat { get; }

    /// <summary>
    /// True to open in a new tab (equivalent to target="_blank" on an anchor tag).
    /// </summary>
    bool OpenInNewWindow { get; }

    string LogoUrl { get; }
}

public class DefaultMessageTemplateEditor : IMessageTemplateEditor
{
    public string Name => "[Default]";

    public string UrlFormat => null;

    public bool OpenInNewWindow => false;

    public string LogoUrl => "/_content/Mantle.Web.Messaging/img/logo-TinyMCE.png";
}

// TODO: Create a separate GrapesJS editor for MJML and one for plain HTML
public class GrapesJsMessageTemplateEditor : IMessageTemplateEditor
{
    public string Name => "Grapes JS";

    public string UrlFormat => "/admin/messaging/grapes-js-templates/edit/{0}/{1}";

    public bool OpenInNewWindow => true;

    public string LogoUrl => "/_content/Mantle.Web.Messaging/img/logo-GrapesJS.png";
}