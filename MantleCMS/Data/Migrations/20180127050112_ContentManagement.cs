using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MantleCMS.Data.Migrations
{
    public partial class ContentManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mantle_BlogCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    UrlSlug = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_BlogCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_BlogTags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    UrlSlug = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_BlogTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_ContentBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BlockName = table.Column<string>(maxLength: 255, nullable: false),
                    BlockType = table.Column<string>(unicode: false, maxLength: 1024, nullable: false),
                    BlockValues = table.Column<string>(nullable: true),
                    CustomTemplatePath = table.Column<string>(maxLength: 255, nullable: true),
                    DisplayCondition = table.Column<string>(maxLength: 255, nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    PageId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    ZoneId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_ContentBlocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_EntityTypeContentBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BlockName = table.Column<string>(maxLength: 255, nullable: false),
                    BlockType = table.Column<string>(unicode: false, maxLength: 1024, nullable: false),
                    BlockValues = table.Column<string>(nullable: true),
                    CustomTemplatePath = table.Column<string>(maxLength: 255, nullable: true),
                    EntityId = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    EntityType = table.Column<string>(unicode: false, maxLength: 512, nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    ZoneId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_EntityTypeContentBlocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_MenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CssClass = table.Column<string>(unicode: false, maxLength: 128, nullable: true),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    IsExternalUrl = table.Column<bool>(nullable: false),
                    MenuId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    RefId = table.Column<Guid>(nullable: true),
                    Text = table.Column<string>(maxLength: 255, nullable: false),
                    Url = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    UrlFilter = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_Pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccessRestrictions = table.Column<string>(unicode: false, maxLength: 1024, nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Order = table.Column<int>(nullable: false),
                    PageTypeId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    ShowOnMenus = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Pages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_PageTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LayoutPath = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_PageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_SitemapConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChangeFrequency = table.Column<byte>(nullable: false),
                    PageId = table.Column<Guid>(nullable: false),
                    Priority = table.Column<float>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_SitemapConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_Zones",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Zones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_BlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    DateCreatedUtc = table.Column<DateTime>(nullable: false),
                    ExternalLink = table.Column<string>(maxLength: 255, nullable: true),
                    FullDescription = table.Column<string>(nullable: true),
                    Headline = table.Column<string>(maxLength: 128, nullable: false),
                    MetaDescription = table.Column<string>(maxLength: 255, nullable: true),
                    MetaKeywords = table.Column<string>(maxLength: 255, nullable: true),
                    ShortDescription = table.Column<string>(nullable: false),
                    Slug = table.Column<string>(maxLength: 128, nullable: false),
                    TeaserImageUrl = table.Column<string>(maxLength: 255, nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UseExternalLink = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_BlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mantle_BlogPosts_Mantle_BlogCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Mantle_BlogCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_PageVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CultureCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    DateCreatedUtc = table.Column<DateTime>(nullable: false),
                    DateModifiedUtc = table.Column<DateTime>(nullable: false),
                    Fields = table.Column<string>(nullable: true),
                    PageId = table.Column<Guid>(nullable: false),
                    Slug = table.Column<string>(maxLength: 255, nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_PageVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mantle_PageVersions_Mantle_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Mantle_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_BlogPostTags",
                columns: table => new
                {
                    PostId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_BlogPostTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_Mantle_BlogPostTags_Mantle_BlogPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "Mantle_BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mantle_BlogPostTags_Mantle_BlogTags_TagId",
                        column: x => x.TagId,
                        principalTable: "Mantle_BlogTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mantle_BlogPosts_CategoryId",
                table: "Mantle_BlogPosts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Mantle_BlogPostTags_TagId",
                table: "Mantle_BlogPostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Mantle_PageVersions_PageId",
                table: "Mantle_PageVersions",
                column: "PageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mantle_BlogPostTags");

            migrationBuilder.DropTable(
                name: "Mantle_ContentBlocks");

            migrationBuilder.DropTable(
                name: "Mantle_EntityTypeContentBlocks");

            migrationBuilder.DropTable(
                name: "Mantle_MenuItems");

            migrationBuilder.DropTable(
                name: "Mantle_Menus");

            migrationBuilder.DropTable(
                name: "Mantle_PageTypes");

            migrationBuilder.DropTable(
                name: "Mantle_PageVersions");

            migrationBuilder.DropTable(
                name: "Mantle_SitemapConfig");

            migrationBuilder.DropTable(
                name: "Mantle_Zones");

            migrationBuilder.DropTable(
                name: "Mantle_BlogPosts");

            migrationBuilder.DropTable(
                name: "Mantle_BlogTags");

            migrationBuilder.DropTable(
                name: "Mantle_Pages");

            migrationBuilder.DropTable(
                name: "Mantle_BlogCategories");
        }
    }
}