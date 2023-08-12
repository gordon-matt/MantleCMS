using Mantle.Identity;
using Mantle.Messaging.Data;
using Mantle.Messaging.Data.Entities;
using Mantle.Web.ContentManagement;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Entities;
using Microsoft.EntityFrameworkCore;

namespace MantleCMS.Data;

public class ApplicationDbContext : MantleIdentityDbContext<ApplicationUser, ApplicationRole>,
    IMantleCmsDbContext,
    IMantleMessagingDbContext
{
    public DbSet<Permission> Permissions { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<UserProfileEntry> UserProfiles { get; set; }

    #region IMantleCmsDbContext Members

    public DbSet<BlogCategory> BlogCategories { get; set; }

    public DbSet<BlogPost> BlogPosts { get; set; }

    public DbSet<BlogPostTag> BlogPostTags { get; set; }

    public DbSet<BlogTag> BlogTags { get; set; }

    public DbSet<ContentBlock> ContentBlocks { get; set; }

    public DbSet<EntityTypeContentBlock> EntityTypeContentBlocks { get; set; }

    public DbSet<Menu> Menus { get; set; }

    public DbSet<MenuItem> MenuItems { get; set; }

    public DbSet<Page> Pages { get; set; }

    public DbSet<PageType> PageTypes { get; set; }

    public DbSet<PageVersion> PageVersions { get; set; }

    public DbSet<SitemapConfig> SitemapConfig { get; set; }

    public DbSet<Zone> Zones { get; set; }

    #endregion IMantleCmsDbContext Members

    #region IMantleMessagingDbContext Members

    public DbSet<MessageTemplate> MessageTemplates { get; set; }

    public DbSet<QueuedEmail> QueuedEmails { get; set; }

    #endregion IMantleMessagingDbContext Members

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}