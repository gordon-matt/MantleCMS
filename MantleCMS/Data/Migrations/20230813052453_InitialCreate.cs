using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantleCMS.Data.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "mantle");
        migrationBuilder.EnsureSchema(name: "plugins");

        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "BlogCategories",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                UrlSlug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlogCategories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "BlogTags",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                UrlSlug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlogTags", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Common_Regions",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                RegionType = table.Column<byte>(type: "tinyint", nullable: false),
                CountryCode = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: true),
                HasStates = table.Column<bool>(type: "bit", nullable: false),
                StateCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                ParentId = table.Column<int>(type: "int", nullable: true),
                Order = table.Column<short>(type: "smallint", nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Common_Regions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Common_Regions_Common_Regions_ParentId",
                    column: x => x.ParentId,
                    principalSchema: "mantle",
                    principalTable: "Common_Regions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Common_RegionSettings",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RegionId = table.Column<int>(type: "int", nullable: false),
                SettingsId = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                Fields = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Common_RegionSettings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ContentBlocks",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                BlockName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                BlockType = table.Column<string>(type: "varchar(1024)", unicode: false, maxLength: 1024, nullable: false),
                Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                BlockValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CustomTemplatePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ContentBlocks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "EntityTypeContentBlocks",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EntityType = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false),
                EntityId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                BlockName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                BlockType = table.Column<string>(type: "varchar(1024)", unicode: false, maxLength: 1024, nullable: false),
                Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                BlockValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CustomTemplatePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EntityTypeContentBlocks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GenericAttributes",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                EntityType = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false),
                EntityId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                Property = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GenericAttributes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Languages",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CultureCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                IsRTL = table.Column<bool>(type: "bit", nullable: false),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                SortOrder = table.Column<int>(type: "int", nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Languages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LocalizableProperties",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CultureCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                EntityType = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false),
                EntityId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                Property = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LocalizableProperties", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LocalizableStrings",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CultureCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                TextKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LocalizableStrings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Log",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                EventDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                EventLevel = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                UserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                MachineName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                EventMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ErrorSource = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ErrorClass = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: true),
                ErrorMethod = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InnerErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Log", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "MenuItems",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CssClass = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                Position = table.Column<int>(type: "int", nullable: false),
                ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                IsExternalUrl = table.Column<bool>(type: "bit", nullable: false),
                RefId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MenuItems", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Menus",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                UrlFilter = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Menus", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "MessageTemplates",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Editor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MessageTemplates", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Pages",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                PageTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                ShowOnMenus = table.Column<bool>(type: "bit", nullable: false),
                AccessRestrictions = table.Column<string>(type: "varchar(1024)", unicode: false, maxLength: 1024, nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Pages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PageTypes",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                LayoutPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PageTypes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Permissions",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Permissions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "QueuedEmails",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Priority = table.Column<int>(type: "int", nullable: false),
                FromAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                FromName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ToAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ToName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                MailMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                SentTries = table.Column<int>(type: "int", nullable: false),
                SentOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QueuedEmails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ScheduledTasks",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Type = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                Seconds = table.Column<int>(type: "int", nullable: false),
                Enabled = table.Column<bool>(type: "bit", nullable: false),
                StopOnError = table.Column<bool>(type: "bit", nullable: false),
                LastStartUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastEndUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastSuccessUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ScheduledTasks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Settings",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Type = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Settings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SitemapConfig",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ChangeFrequency = table.Column<byte>(type: "tinyint", nullable: false),
                Priority = table.Column<float>(type: "real", nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SitemapConfig", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tenants",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Hosts = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserProfiles",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserProfiles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Zones",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zones", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ApplicationRoleId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUsers_AspNetRoles_ApplicationRoleId",
                    column: x => x.ApplicationRoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "BlogPosts",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                DateCreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CategoryId = table.Column<int>(type: "int", nullable: false),
                Headline = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Slug = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                TeaserImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UseExternalLink = table.Column<bool>(type: "bit", nullable: false),
                ExternalLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                MetaKeywords = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                MetaDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlogPosts", x => x.Id);
                table.ForeignKey(
                    name: "FK_BlogPosts_BlogCategories_CategoryId",
                    column: x => x.CategoryId,
                    principalSchema: "mantle",
                    principalTable: "BlogCategories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "MessageTemplateVersions",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MessageTemplateId = table.Column<int>(type: "int", nullable: false),
                CultureCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DateCreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                DateModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MessageTemplateVersions", x => x.Id);
                table.ForeignKey(
                    name: "FK_MessageTemplateVersions_MessageTemplates_MessageTemplateId",
                    column: x => x.MessageTemplateId,
                    principalSchema: "mantle",
                    principalTable: "MessageTemplates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PageVersions",
            schema: "mantle",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CultureCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                DateCreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                DateModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status = table.Column<byte>(type: "tinyint", nullable: false),
                Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Fields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PageVersions", x => x.Id);
                table.ForeignKey(
                    name: "FK_PageVersions_Pages_PageId",
                    column: x => x.PageId,
                    principalSchema: "mantle",
                    principalTable: "Pages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RolePermissions",
            schema: "mantle",
            columns: table => new
            {
                PermissionId = table.Column<int>(type: "int", nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermissions", x => new { x.PermissionId, x.RoleId });
                table.ForeignKey(
                    name: "FK_RolePermissions_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RolePermissions_Permissions_PermissionId",
                    column: x => x.PermissionId,
                    principalSchema: "mantle",
                    principalTable: "Permissions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "BlogPostTags",
            schema: "mantle",
            columns: table => new
            {
                PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TagId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlogPostTags", x => new { x.PostId, x.TagId });
                table.ForeignKey(
                    name: "FK_BlogPostTags_BlogPosts_PostId",
                    column: x => x.PostId,
                    principalSchema: "mantle",
                    principalTable: "BlogPosts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_BlogPostTags_BlogTags_TagId",
                    column: x => x.TagId,
                    principalSchema: "mantle",
                    principalTable: "BlogTags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_ApplicationRoleId",
            table: "AspNetUsers",
            column: "ApplicationRoleId");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_BlogPosts_CategoryId",
            schema: "mantle",
            table: "BlogPosts",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_BlogPostTags_TagId",
            schema: "mantle",
            table: "BlogPostTags",
            column: "TagId");

        migrationBuilder.CreateIndex(
            name: "IX_Common_Regions_ParentId",
            schema: "mantle",
            table: "Common_Regions",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_MessageTemplateVersions_MessageTemplateId",
            schema: "mantle",
            table: "MessageTemplateVersions",
            column: "MessageTemplateId");

        migrationBuilder.CreateIndex(
            name: "IX_PageVersions_PageId",
            schema: "mantle",
            table: "PageVersions",
            column: "PageId");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_RoleId",
            schema: "mantle",
            table: "RolePermissions",
            column: "RoleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "BlogPostTags",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Common_Regions",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Common_RegionSettings",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "ContentBlocks",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "EntityTypeContentBlocks",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "GenericAttributes",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Languages",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "LocalizableProperties",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "LocalizableStrings",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Log",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "MenuItems",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Menus",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "MessageTemplateVersions",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "PageTypes",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "PageVersions",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "QueuedEmails",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "RolePermissions",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "ScheduledTasks",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Settings",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "SitemapConfig",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Tenants",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "UserProfiles",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Zones",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "AspNetUsers");

        migrationBuilder.DropTable(
            name: "BlogPosts",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "BlogTags",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "MessageTemplates",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Pages",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "Permissions",
            schema: "mantle");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "BlogCategories",
            schema: "mantle");

        migrationBuilder.DropSchema(name: "plugins");
        migrationBuilder.DropSchema(name: "mantle");
    }
}