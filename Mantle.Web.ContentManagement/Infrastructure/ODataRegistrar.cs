using Extenso.AspNetCore.OData;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Controllers.Api;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers.Api;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Mantle.Web.ContentManagement.Infrastructure;

public class ODataRegistrar : IODataRegistrar
{
    #region IODataRegistrar Members

    public void Register(ODataOptions options)
    {
        ODataModelBuilder builder = new ODataConventionModelBuilder();

        // Blog
        builder.EntitySet<BlogCategory>("BlogCategoryApi");
        builder.EntitySet<BlogPost>("BlogPostApi");
        builder.EntitySet<BlogPostTag>("BlogPostTagApi").EntityType.HasKey(x => new { x.PostId, x.TagId });
        builder.EntitySet<BlogTag>("BlogTagApi");

        // Content Blocks
        builder.EntitySet<ContentBlock>("ContentBlockApi");
        builder.EntitySet<EntityTypeContentBlock>("EntityTypeContentBlockApi");
        builder.EntitySet<Zone>("ZoneApi");

        // Menus
        builder.EntitySet<Menu>("MenuApi");
        builder.EntitySet<MenuItem>("MenuItemApi");

        // Pages
        builder.EntitySet<Page>("PageApi");
        builder.EntitySet<PageType>("PageTypeApi");
        builder.EntitySet<PageVersion>("PageVersionApi");
        builder.EntitySet<PageTreeItem>("PageTreeApi");

        // Other
        builder.EntitySet<SitemapConfig>("XmlSitemapApi");
        builder.EntitySet<Subscriber>("SubscriberApi");

        // Action Configurations
        RegisterContentBlockODataActions(builder);
        RegisterEntityTypeContentBlockODataActions(builder);
        RegisterPageODataActions(builder);
        RegisterPageVersionODataActions(builder);
        RegisterXmlSitemapODataActions(builder);

        builder.EntityType<SitemapConfig>().HasKey(x => x.Id);

        options.AddRouteComponents("odata/mantle/cms", builder.GetEdmModel());
    }

    #endregion IODataRegistrar Members

    private static void RegisterContentBlockODataActions(ODataModelBuilder builder)
    {
        var getByPageIdFunction = builder.EntityType<ContentBlock>().Collection.Function("GetByPageId");
        getByPageIdFunction.Parameter<Guid>("pageId");
        getByPageIdFunction.ReturnsCollectionFromEntitySet<ContentBlock>("ContentBlockApi");

        var getLocalizedActionFunction = builder.EntityType<ContentBlock>().Collection.Function("GetLocalized");
        getLocalizedActionFunction.Parameter<Guid>("id");
        getLocalizedActionFunction.Parameter<string>("cultureCode");
        getLocalizedActionFunction.ReturnsFromEntitySet<ContentBlock>("ContentBlockApi");

        var saveLocalizedAction = builder.EntityType<ContentBlock>().Collection.Action("SaveLocalized");
        saveLocalizedAction.Parameter<string>("cultureCode");
        saveLocalizedAction.Parameter<ContentBlock>("entity");
        saveLocalizedAction.Returns<IActionResult>();
    }

    private static void RegisterEntityTypeContentBlockODataActions(ODataModelBuilder builder)
    {
        var getLocalizedActionFunction = builder.EntityType<EntityTypeContentBlock>().Collection.Function("GetLocalized");
        getLocalizedActionFunction.Parameter<Guid>("id");
        getLocalizedActionFunction.Parameter<string>("cultureCode");
        getLocalizedActionFunction.ReturnsFromEntitySet<EntityTypeContentBlock>("EntityTypeContentBlockApi");

        var saveLocalizedAction = builder.EntityType<EntityTypeContentBlock>().Collection.Action("SaveLocalized");
        saveLocalizedAction.Parameter<string>("cultureCode");
        saveLocalizedAction.Parameter<EntityTypeContentBlock>("entity");
        saveLocalizedAction.Returns<IActionResult>();
    }

    private static void RegisterPageODataActions(ODataModelBuilder builder)
    {
        var getTopLevelPagesFunction = builder.EntityType<Page>().Collection.Function("GetTopLevelPages");
        getTopLevelPagesFunction.ReturnsFromEntitySet<Page>("PageApi");
    }

    private static void RegisterPageVersionODataActions(ODataModelBuilder builder)
    {
        var restoreVersionAction = builder.EntityType<PageVersion>().Action("RestoreVersion");
        restoreVersionAction.Returns<IActionResult>();

        var getCurrentVersionFunction = builder.EntityType<PageVersion>().Collection.Function("GetCurrentVersion");
        getCurrentVersionFunction.Parameter<Guid>("pageId");
        getCurrentVersionFunction.Parameter<string>("cultureCode");
        getCurrentVersionFunction.Returns<EdmPageVersion>();
    }

    private static void RegisterXmlSitemapODataActions(ODataModelBuilder builder)
    {
        var getConfigFunction = builder.EntityType<SitemapConfig>().Collection.Function("GetConfig");
        getConfigFunction.ReturnsCollection<SitemapConfigModel>();

        var setConfigAction = builder.EntityType<SitemapConfig>().Collection.Action("SetConfig");
        setConfigAction.Parameter<int>("id");
        setConfigAction.Parameter<byte>("changeFrequency");
        setConfigAction.Parameter<float>("priority");
        setConfigAction.Returns<IActionResult>();

        var generateAction = builder.EntityType<SitemapConfig>().Collection.Action("Generate");
        generateAction.Returns<IActionResult>();
    }
}