using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantleCMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncLatestSchema20230812 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mantle_BlogPosts_Mantle_BlogCategories_CategoryId",
                table: "Mantle_BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Mantle_BlogPostTags_Mantle_BlogPosts_PostId",
                table: "Mantle_BlogPostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Mantle_BlogPostTags_Mantle_BlogTags_TagId",
                table: "Mantle_BlogPostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Mantle_Common_Regions_Mantle_Common_Regions_ParentId",
                table: "Mantle_Common_Regions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mantle_MessageTemplateVersions_Mantle_MessageTemplates_MessageTemplateId",
                table: "Mantle_MessageTemplateVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mantle_PageVersions_Mantle_Pages_PageId",
                table: "Mantle_PageVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Zones",
                table: "Mantle_Zones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Tenants",
                table: "Mantle_Tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_SitemapConfig",
                table: "Mantle_SitemapConfig");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Settings",
                table: "Mantle_Settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_ScheduledTasks",
                table: "Mantle_ScheduledTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_QueuedEmails",
                table: "Mantle_QueuedEmails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_PageVersions",
                table: "Mantle_PageVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_PageTypes",
                table: "Mantle_PageTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Pages",
                table: "Mantle_Pages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_MessageTemplateVersions",
                table: "Mantle_MessageTemplateVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_MessageTemplates",
                table: "Mantle_MessageTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Menus",
                table: "Mantle_Menus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_MenuItems",
                table: "Mantle_MenuItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Log",
                table: "Mantle_Log");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_LocalizableStrings",
                table: "Mantle_LocalizableStrings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_LocalizableProperties",
                table: "Mantle_LocalizableProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Languages",
                table: "Mantle_Languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_GenericAttributes",
                table: "Mantle_GenericAttributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_EntityTypeContentBlocks",
                table: "Mantle_EntityTypeContentBlocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_ContentBlocks",
                table: "Mantle_ContentBlocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Common_RegionSettings",
                table: "Mantle_Common_RegionSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_Common_Regions",
                table: "Mantle_Common_Regions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_BlogTags",
                table: "Mantle_BlogTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_BlogPostTags",
                table: "Mantle_BlogPostTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_BlogPosts",
                table: "Mantle_BlogPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mantle_BlogCategories",
                table: "Mantle_BlogCategories");

            migrationBuilder.EnsureSchema(
                name: "mantle");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                newName: "UserProfiles",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "RolePermissions",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Permissions",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Zones",
                newName: "Zones",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Tenants",
                newName: "Tenants",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_SitemapConfig",
                newName: "SitemapConfig",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Settings",
                newName: "Settings",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_ScheduledTasks",
                newName: "ScheduledTasks",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_QueuedEmails",
                newName: "QueuedEmails",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_PageVersions",
                newName: "PageVersions",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_PageTypes",
                newName: "PageTypes",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Pages",
                newName: "Pages",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_MessageTemplateVersions",
                newName: "MessageTemplateVersions",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_MessageTemplates",
                newName: "MessageTemplates",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Menus",
                newName: "Menus",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_MenuItems",
                newName: "MenuItems",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Log",
                newName: "Log",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_LocalizableStrings",
                newName: "LocalizableStrings",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_LocalizableProperties",
                newName: "LocalizableProperties",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Languages",
                newName: "Languages",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_GenericAttributes",
                newName: "GenericAttributes",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_EntityTypeContentBlocks",
                newName: "EntityTypeContentBlocks",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_ContentBlocks",
                newName: "ContentBlocks",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Common_RegionSettings",
                newName: "Common_RegionSettings",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_Common_Regions",
                newName: "Common_Regions",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_BlogTags",
                newName: "BlogTags",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_BlogPostTags",
                newName: "BlogPostTags",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_BlogPosts",
                newName: "BlogPosts",
                newSchema: "mantle");

            migrationBuilder.RenameTable(
                name: "Mantle_BlogCategories",
                newName: "BlogCategories",
                newSchema: "mantle");

            migrationBuilder.RenameIndex(
                name: "IX_Mantle_PageVersions_PageId",
                schema: "mantle",
                table: "PageVersions",
                newName: "IX_PageVersions_PageId");

            migrationBuilder.RenameIndex(
                name: "IX_Mantle_MessageTemplateVersions_MessageTemplateId",
                schema: "mantle",
                table: "MessageTemplateVersions",
                newName: "IX_MessageTemplateVersions_MessageTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Mantle_Common_Regions_ParentId",
                schema: "mantle",
                table: "Common_Regions",
                newName: "IX_Common_Regions_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Mantle_BlogPostTags_TagId",
                schema: "mantle",
                table: "BlogPostTags",
                newName: "IX_BlogPostTags_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_Mantle_BlogPosts_CategoryId",
                schema: "mantle",
                table: "BlogPosts",
                newName: "IX_BlogPosts_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Zones",
                schema: "mantle",
                table: "Zones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenants",
                schema: "mantle",
                table: "Tenants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SitemapConfig",
                schema: "mantle",
                table: "SitemapConfig",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                schema: "mantle",
                table: "Settings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduledTasks",
                schema: "mantle",
                table: "ScheduledTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueuedEmails",
                schema: "mantle",
                table: "QueuedEmails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PageVersions",
                schema: "mantle",
                table: "PageVersions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PageTypes",
                schema: "mantle",
                table: "PageTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pages",
                schema: "mantle",
                table: "Pages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageTemplateVersions",
                schema: "mantle",
                table: "MessageTemplateVersions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageTemplates",
                schema: "mantle",
                table: "MessageTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Menus",
                schema: "mantle",
                table: "Menus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItems",
                schema: "mantle",
                table: "MenuItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Log",
                schema: "mantle",
                table: "Log",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocalizableStrings",
                schema: "mantle",
                table: "LocalizableStrings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocalizableProperties",
                schema: "mantle",
                table: "LocalizableProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Languages",
                schema: "mantle",
                table: "Languages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GenericAttributes",
                schema: "mantle",
                table: "GenericAttributes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityTypeContentBlocks",
                schema: "mantle",
                table: "EntityTypeContentBlocks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentBlocks",
                schema: "mantle",
                table: "ContentBlocks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Common_RegionSettings",
                schema: "mantle",
                table: "Common_RegionSettings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Common_Regions",
                schema: "mantle",
                table: "Common_Regions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogTags",
                schema: "mantle",
                table: "BlogTags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPostTags",
                schema: "mantle",
                table: "BlogPostTags",
                columns: new[] { "PostId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPosts",
                schema: "mantle",
                table: "BlogPosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogCategories",
                schema: "mantle",
                table: "BlogCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_BlogCategories_CategoryId",
                schema: "mantle",
                table: "BlogPosts",
                column: "CategoryId",
                principalSchema: "mantle",
                principalTable: "BlogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTags_BlogPosts_PostId",
                schema: "mantle",
                table: "BlogPostTags",
                column: "PostId",
                principalSchema: "mantle",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTags_BlogTags_TagId",
                schema: "mantle",
                table: "BlogPostTags",
                column: "TagId",
                principalSchema: "mantle",
                principalTable: "BlogTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Common_Regions_Common_Regions_ParentId",
                schema: "mantle",
                table: "Common_Regions",
                column: "ParentId",
                principalSchema: "mantle",
                principalTable: "Common_Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTemplateVersions_MessageTemplates_MessageTemplateId",
                schema: "mantle",
                table: "MessageTemplateVersions",
                column: "MessageTemplateId",
                principalSchema: "mantle",
                principalTable: "MessageTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageVersions_Pages_PageId",
                schema: "mantle",
                table: "PageVersions",
                column: "PageId",
                principalSchema: "mantle",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_BlogCategories_CategoryId",
                schema: "mantle",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTags_BlogPosts_PostId",
                schema: "mantle",
                table: "BlogPostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTags_BlogTags_TagId",
                schema: "mantle",
                table: "BlogPostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Common_Regions_Common_Regions_ParentId",
                schema: "mantle",
                table: "Common_Regions");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageTemplateVersions_MessageTemplates_MessageTemplateId",
                schema: "mantle",
                table: "MessageTemplateVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_PageVersions_Pages_PageId",
                schema: "mantle",
                table: "PageVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Zones",
                schema: "mantle",
                table: "Zones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenants",
                schema: "mantle",
                table: "Tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SitemapConfig",
                schema: "mantle",
                table: "SitemapConfig");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                schema: "mantle",
                table: "Settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduledTasks",
                schema: "mantle",
                table: "ScheduledTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueuedEmails",
                schema: "mantle",
                table: "QueuedEmails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PageVersions",
                schema: "mantle",
                table: "PageVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PageTypes",
                schema: "mantle",
                table: "PageTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pages",
                schema: "mantle",
                table: "Pages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageTemplateVersions",
                schema: "mantle",
                table: "MessageTemplateVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageTemplates",
                schema: "mantle",
                table: "MessageTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Menus",
                schema: "mantle",
                table: "Menus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItems",
                schema: "mantle",
                table: "MenuItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Log",
                schema: "mantle",
                table: "Log");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocalizableStrings",
                schema: "mantle",
                table: "LocalizableStrings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocalizableProperties",
                schema: "mantle",
                table: "LocalizableProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Languages",
                schema: "mantle",
                table: "Languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GenericAttributes",
                schema: "mantle",
                table: "GenericAttributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityTypeContentBlocks",
                schema: "mantle",
                table: "EntityTypeContentBlocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentBlocks",
                schema: "mantle",
                table: "ContentBlocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Common_RegionSettings",
                schema: "mantle",
                table: "Common_RegionSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Common_Regions",
                schema: "mantle",
                table: "Common_Regions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogTags",
                schema: "mantle",
                table: "BlogTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPostTags",
                schema: "mantle",
                table: "BlogPostTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPosts",
                schema: "mantle",
                table: "BlogPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogCategories",
                schema: "mantle",
                table: "BlogCategories");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                schema: "mantle",
                newName: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                schema: "mantle",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                schema: "mantle",
                newName: "Permissions");

            migrationBuilder.RenameTable(
                name: "Zones",
                schema: "mantle",
                newName: "Mantle_Zones");

            migrationBuilder.RenameTable(
                name: "Tenants",
                schema: "mantle",
                newName: "Mantle_Tenants");

            migrationBuilder.RenameTable(
                name: "SitemapConfig",
                schema: "mantle",
                newName: "Mantle_SitemapConfig");

            migrationBuilder.RenameTable(
                name: "Settings",
                schema: "mantle",
                newName: "Mantle_Settings");

            migrationBuilder.RenameTable(
                name: "ScheduledTasks",
                schema: "mantle",
                newName: "Mantle_ScheduledTasks");

            migrationBuilder.RenameTable(
                name: "QueuedEmails",
                schema: "mantle",
                newName: "Mantle_QueuedEmails");

            migrationBuilder.RenameTable(
                name: "PageVersions",
                schema: "mantle",
                newName: "Mantle_PageVersions");

            migrationBuilder.RenameTable(
                name: "PageTypes",
                schema: "mantle",
                newName: "Mantle_PageTypes");

            migrationBuilder.RenameTable(
                name: "Pages",
                schema: "mantle",
                newName: "Mantle_Pages");

            migrationBuilder.RenameTable(
                name: "MessageTemplateVersions",
                schema: "mantle",
                newName: "Mantle_MessageTemplateVersions");

            migrationBuilder.RenameTable(
                name: "MessageTemplates",
                schema: "mantle",
                newName: "Mantle_MessageTemplates");

            migrationBuilder.RenameTable(
                name: "Menus",
                schema: "mantle",
                newName: "Mantle_Menus");

            migrationBuilder.RenameTable(
                name: "MenuItems",
                schema: "mantle",
                newName: "Mantle_MenuItems");

            migrationBuilder.RenameTable(
                name: "Log",
                schema: "mantle",
                newName: "Mantle_Log");

            migrationBuilder.RenameTable(
                name: "LocalizableStrings",
                schema: "mantle",
                newName: "Mantle_LocalizableStrings");

            migrationBuilder.RenameTable(
                name: "LocalizableProperties",
                schema: "mantle",
                newName: "Mantle_LocalizableProperties");

            migrationBuilder.RenameTable(
                name: "Languages",
                schema: "mantle",
                newName: "Mantle_Languages");

            migrationBuilder.RenameTable(
                name: "GenericAttributes",
                schema: "mantle",
                newName: "Mantle_GenericAttributes");

            migrationBuilder.RenameTable(
                name: "EntityTypeContentBlocks",
                schema: "mantle",
                newName: "Mantle_EntityTypeContentBlocks");

            migrationBuilder.RenameTable(
                name: "ContentBlocks",
                schema: "mantle",
                newName: "Mantle_ContentBlocks");

            migrationBuilder.RenameTable(
                name: "Common_RegionSettings",
                schema: "mantle",
                newName: "Mantle_Common_RegionSettings");

            migrationBuilder.RenameTable(
                name: "Common_Regions",
                schema: "mantle",
                newName: "Mantle_Common_Regions");

            migrationBuilder.RenameTable(
                name: "BlogTags",
                schema: "mantle",
                newName: "Mantle_BlogTags");

            migrationBuilder.RenameTable(
                name: "BlogPostTags",
                schema: "mantle",
                newName: "Mantle_BlogPostTags");

            migrationBuilder.RenameTable(
                name: "BlogPosts",
                schema: "mantle",
                newName: "Mantle_BlogPosts");

            migrationBuilder.RenameTable(
                name: "BlogCategories",
                schema: "mantle",
                newName: "Mantle_BlogCategories");

            migrationBuilder.RenameIndex(
                name: "IX_PageVersions_PageId",
                table: "Mantle_PageVersions",
                newName: "IX_Mantle_PageVersions_PageId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageTemplateVersions_MessageTemplateId",
                table: "Mantle_MessageTemplateVersions",
                newName: "IX_Mantle_MessageTemplateVersions_MessageTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Common_Regions_ParentId",
                table: "Mantle_Common_Regions",
                newName: "IX_Mantle_Common_Regions_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostTags_TagId",
                table: "Mantle_BlogPostTags",
                newName: "IX_Mantle_BlogPostTags_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_CategoryId",
                table: "Mantle_BlogPosts",
                newName: "IX_Mantle_BlogPosts_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Zones",
                table: "Mantle_Zones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Tenants",
                table: "Mantle_Tenants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_SitemapConfig",
                table: "Mantle_SitemapConfig",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Settings",
                table: "Mantle_Settings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_ScheduledTasks",
                table: "Mantle_ScheduledTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_QueuedEmails",
                table: "Mantle_QueuedEmails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_PageVersions",
                table: "Mantle_PageVersions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_PageTypes",
                table: "Mantle_PageTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Pages",
                table: "Mantle_Pages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_MessageTemplateVersions",
                table: "Mantle_MessageTemplateVersions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_MessageTemplates",
                table: "Mantle_MessageTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Menus",
                table: "Mantle_Menus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_MenuItems",
                table: "Mantle_MenuItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Log",
                table: "Mantle_Log",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_LocalizableStrings",
                table: "Mantle_LocalizableStrings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_LocalizableProperties",
                table: "Mantle_LocalizableProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Languages",
                table: "Mantle_Languages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_GenericAttributes",
                table: "Mantle_GenericAttributes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_EntityTypeContentBlocks",
                table: "Mantle_EntityTypeContentBlocks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_ContentBlocks",
                table: "Mantle_ContentBlocks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Common_RegionSettings",
                table: "Mantle_Common_RegionSettings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_Common_Regions",
                table: "Mantle_Common_Regions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_BlogTags",
                table: "Mantle_BlogTags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_BlogPostTags",
                table: "Mantle_BlogPostTags",
                columns: new[] { "PostId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_BlogPosts",
                table: "Mantle_BlogPosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mantle_BlogCategories",
                table: "Mantle_BlogCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mantle_BlogPosts_Mantle_BlogCategories_CategoryId",
                table: "Mantle_BlogPosts",
                column: "CategoryId",
                principalTable: "Mantle_BlogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mantle_BlogPostTags_Mantle_BlogPosts_PostId",
                table: "Mantle_BlogPostTags",
                column: "PostId",
                principalTable: "Mantle_BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mantle_BlogPostTags_Mantle_BlogTags_TagId",
                table: "Mantle_BlogPostTags",
                column: "TagId",
                principalTable: "Mantle_BlogTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mantle_Common_Regions_Mantle_Common_Regions_ParentId",
                table: "Mantle_Common_Regions",
                column: "ParentId",
                principalTable: "Mantle_Common_Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mantle_MessageTemplateVersions_Mantle_MessageTemplates_MessageTemplateId",
                table: "Mantle_MessageTemplateVersions",
                column: "MessageTemplateId",
                principalTable: "Mantle_MessageTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mantle_PageVersions_Mantle_Pages_PageId",
                table: "Mantle_PageVersions",
                column: "PageId",
                principalTable: "Mantle_Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
