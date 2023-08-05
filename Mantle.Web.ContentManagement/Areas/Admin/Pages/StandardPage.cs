using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;

//using Mantle.Web.Indexing;
using Newtonsoft.Json.Linq;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages;

public class StandardPage : MantlePageType
{
    public override string Name => "Standard Page";

    public override bool IsEnabled => true;

    public override string DisplayTemplatePath => "/Areas/Admin/Pages/Views/Shared/DisplayTemplates/StandardPage.cshtml";

    public override string EditorTemplatePath => "/Areas/Admin/Pages/Views/Shared/EditorTemplates/StandardPage.cshtml";

    public override void InitializeInstance(PageVersion pageVersion)
    {
        if (pageVersion == null || string.IsNullOrEmpty(pageVersion.Fields))
        {
            return;
        }

        dynamic fields = JObject.Parse(pageVersion.Fields);
        MetaTitle = fields.MetaTitle;
        MetaKeywords = fields.MetaKeywords;
        MetaDescription = fields.MetaDescription;
        BodyContent = fields.BodyContent;
    }

    //public override void PopulateDocumentIndex(IDocumentIndex document, out string description)
    //{
    //    document.Add("title", InstanceName).Analyze().Store();
    //    document.Add("meta_title", MetaTitle).Analyze();
    //    document.Add("meta_keywords", MetaKeywords).Analyze();
    //    document.Add("meta_description", MetaDescription).Analyze();
    //    document.Add("body", BodyContent).Analyze().Store();
    //    description = BodyContent;
    //}

    public override void ReplaceContentTokens(Func<string, string> func)
    {
        BodyContent = func(BodyContent);
    }

    //[Searchable]
    public string MetaTitle { get; set; }

    //[Searchable]
    public string MetaKeywords { get; set; }

    //[Searchable]
    public string MetaDescription { get; set; }

    //[Searchable]
    public string BodyContent { get; set; }
}