using Microsoft.EntityFrameworkCore.Migrations;

namespace MantleCMS.Data.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<string>(nullable: false),
                Name = table.Column<string>(maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(nullable: true),
                TenantId = table.Column<int>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_BlogCategories",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
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
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                UrlSlug = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_BlogTags", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_Common_Regions",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                RegionType = table.Column<byte>(nullable: false),
                CountryCode = table.Column<string>(unicode: false, maxLength: 2, nullable: true),
                HasStates = table.Column<bool>(nullable: false),
                StateCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                ParentId = table.Column<int>(nullable: true),
                Order = table.Column<short>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_Common_Regions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Mantle_Common_Regions_Mantle_Common_Regions_ParentId",
                    column: x => x.ParentId,
                    principalTable: "Mantle_Common_Regions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_Common_RegionSettings",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RegionId = table.Column<int>(nullable: false),
                SettingsId = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                Fields = table.Column<string>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_Common_RegionSettings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_ContentBlocks",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                BlockName = table.Column<string>(maxLength: 255, nullable: false),
                BlockType = table.Column<string>(unicode: false, maxLength: 1024, nullable: false),
                Title = table.Column<string>(maxLength: 255, nullable: false),
                ZoneId = table.Column<Guid>(nullable: false),
                Order = table.Column<int>(nullable: false),
                IsEnabled = table.Column<bool>(nullable: false),
                BlockValues = table.Column<string>(nullable: true),
                CustomTemplatePath = table.Column<string>(maxLength: 255, nullable: true),
                PageId = table.Column<Guid>(nullable: true)
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
                EntityType = table.Column<string>(unicode: false, maxLength: 512, nullable: false),
                EntityId = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                BlockName = table.Column<string>(maxLength: 255, nullable: false),
                BlockType = table.Column<string>(unicode: false, maxLength: 1024, nullable: false),
                Title = table.Column<string>(maxLength: 255, nullable: false),
                ZoneId = table.Column<Guid>(nullable: false),
                Order = table.Column<int>(nullable: false),
                IsEnabled = table.Column<bool>(nullable: false),
                BlockValues = table.Column<string>(nullable: true),
                CustomTemplatePath = table.Column<string>(maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_EntityTypeContentBlocks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_GenericAttributes",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                EntityType = table.Column<string>(unicode: false, maxLength: 512, nullable: false),
                EntityId = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                Property = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                Value = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_GenericAttributes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_Languages",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                CultureCode = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                IsRTL = table.Column<bool>(nullable: false),
                IsEnabled = table.Column<bool>(nullable: false),
                SortOrder = table.Column<int>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_Languages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_LocalizableProperties",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CultureCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                EntityType = table.Column<string>(unicode: false, maxLength: 512, nullable: false),
                EntityId = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                Property = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                Value = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_LocalizableProperties", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_LocalizableStrings",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                CultureCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                TextKey = table.Column<string>(nullable: false),
                TextValue = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_LocalizableStrings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_Log",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(nullable: true),
                EventDateTime = table.Column<DateTime>(nullable: false),
                EventLevel = table.Column<string>(unicode: false, maxLength: 5, nullable: false),
                UserName = table.Column<string>(maxLength: 128, nullable: false),
                MachineName = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                EventMessage = table.Column<string>(nullable: false),
                ErrorSource = table.Column<string>(maxLength: 255, nullable: false),
                ErrorClass = table.Column<string>(unicode: false, maxLength: 512, nullable: true),
                ErrorMethod = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                ErrorMessage = table.Column<string>(nullable: true),
                InnerErrorMessage = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_Log", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_MenuItems",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                MenuId = table.Column<Guid>(nullable: false),
                Text = table.Column<string>(maxLength: 255, nullable: false),
                Description = table.Column<string>(maxLength: 255, nullable: true),
                Url = table.Column<string>(maxLength: 255, nullable: false),
                CssClass = table.Column<string>(unicode: false, maxLength: 128, nullable: true),
                Position = table.Column<int>(nullable: false),
                ParentId = table.Column<Guid>(nullable: true),
                Enabled = table.Column<bool>(nullable: false),
                IsExternalUrl = table.Column<bool>(nullable: false),
                RefId = table.Column<Guid>(nullable: true)
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
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                UrlFilter = table.Column<string>(maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_Menus", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_MessageTemplates",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                Editor = table.Column<string>(maxLength: 255, nullable: false),
                OwnerId = table.Column<Guid>(nullable: true),
                Enabled = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_MessageTemplates", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_Pages",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                ParentId = table.Column<Guid>(nullable: true),
                PageTypeId = table.Column<Guid>(nullable: false),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                IsEnabled = table.Column<bool>(nullable: false),
                Order = table.Column<int>(nullable: false),
                ShowOnMenus = table.Column<bool>(nullable: false),
                AccessRestrictions = table.Column<string>(unicode: false, maxLength: 1024, nullable: true)
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
                Name = table.Column<string>(maxLength: 255, nullable: false),
                LayoutPath = table.Column<string>(maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_PageTypes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_QueuedEmails",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                Priority = table.Column<int>(nullable: false),
                FromAddress = table.Column<string>(maxLength: 255, nullable: true),
                FromName = table.Column<string>(maxLength: 255, nullable: true),
                ToAddress = table.Column<string>(maxLength: 255, nullable: false),
                ToName = table.Column<string>(maxLength: 255, nullable: true),
                Subject = table.Column<string>(maxLength: 255, nullable: true),
                MailMessage = table.Column<string>(nullable: false),
                CreatedOnUtc = table.Column<DateTime>(nullable: false),
                SentTries = table.Column<int>(nullable: false),
                SentOnUtc = table.Column<DateTime>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_QueuedEmails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_ScheduledTasks",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                Type = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                Seconds = table.Column<int>(nullable: false),
                Enabled = table.Column<bool>(nullable: false),
                StopOnError = table.Column<bool>(nullable: false),
                LastStartUtc = table.Column<DateTime>(nullable: true),
                LastEndUtc = table.Column<DateTime>(nullable: true),
                LastSuccessUtc = table.Column<DateTime>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_ScheduledTasks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_Settings",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                Type = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                Value = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_Settings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_SitemapConfig",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(nullable: true),
                PageId = table.Column<Guid>(nullable: false),
                ChangeFrequency = table.Column<byte>(nullable: false),
                Priority = table.Column<float>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_SitemapConfig", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_Tenants",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                Url = table.Column<string>(maxLength: 255, nullable: false),
                Hosts = table.Column<string>(maxLength: 1024, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_Tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_Zones",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_Zones", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Permissions",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(nullable: true),
                Name = table.Column<string>(maxLength: 50, nullable: false),
                Category = table.Column<string>(maxLength: 50, nullable: false),
                Description = table.Column<string>(maxLength: 128, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Permissions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserProfiles",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(nullable: true),
                UserId = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                Key = table.Column<string>(maxLength: 255, nullable: false),
                Value = table.Column<string>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserProfiles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoleId = table.Column<string>(nullable: false),
                ClaimType = table.Column<string>(nullable: true),
                ClaimValue = table.Column<string>(nullable: true)
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
                Id = table.Column<string>(nullable: false),
                UserName = table.Column<string>(maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                Email = table.Column<string>(maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(nullable: false),
                PasswordHash = table.Column<string>(nullable: true),
                SecurityStamp = table.Column<string>(nullable: true),
                ConcurrencyStamp = table.Column<string>(nullable: true),
                PhoneNumber = table.Column<string>(nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                TwoFactorEnabled = table.Column<bool>(nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                LockoutEnabled = table.Column<bool>(nullable: false),
                AccessFailedCount = table.Column<int>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                ApplicationRoleId = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUsers_AspNetRoles_ApplicationRoleId",
                    column: x => x.ApplicationRoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_BlogPosts",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                UserId = table.Column<string>(maxLength: 128, nullable: false),
                DateCreatedUtc = table.Column<DateTime>(nullable: false),
                CategoryId = table.Column<int>(nullable: false),
                Headline = table.Column<string>(maxLength: 128, nullable: false),
                Slug = table.Column<string>(maxLength: 128, nullable: false),
                TeaserImageUrl = table.Column<string>(maxLength: 255, nullable: true),
                ShortDescription = table.Column<string>(nullable: false),
                FullDescription = table.Column<string>(nullable: true),
                UseExternalLink = table.Column<bool>(nullable: false),
                ExternalLink = table.Column<string>(maxLength: 255, nullable: true),
                MetaKeywords = table.Column<string>(maxLength: 255, nullable: true),
                MetaDescription = table.Column<string>(maxLength: 255, nullable: true)
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
            name: "Mantle_MessageTemplateVersions",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MessageTemplateId = table.Column<int>(nullable: false),
                CultureCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                Subject = table.Column<string>(maxLength: 255, nullable: false),
                Data = table.Column<string>(nullable: true),
                DateCreatedUtc = table.Column<DateTime>(nullable: false),
                DateModifiedUtc = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Mantle_MessageTemplateVersions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Mantle_MessageTemplateVersions_Mantle_MessageTemplates_MessageTemplateId",
                    column: x => x.MessageTemplateId,
                    principalTable: "Mantle_MessageTemplates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Mantle_PageVersions",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                TenantId = table.Column<int>(nullable: true),
                PageId = table.Column<Guid>(nullable: false),
                CultureCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                DateCreatedUtc = table.Column<DateTime>(nullable: false),
                DateModifiedUtc = table.Column<DateTime>(nullable: false),
                Status = table.Column<byte>(nullable: false),
                Title = table.Column<string>(maxLength: 255, nullable: false),
                Slug = table.Column<string>(maxLength: 255, nullable: false),
                Fields = table.Column<string>(nullable: true)
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
            name: "RolePermissions",
            columns: table => new
            {
                PermissionId = table.Column<int>(nullable: false),
                RoleId = table.Column<string>(maxLength: 450, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermissions", x => new { x.PermissionId, x.RoleId });
                table.ForeignKey(
                    name: "FK_RolePermissions_Permissions_PermissionId",
                    column: x => x.PermissionId,
                    principalTable: "Permissions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RolePermissions_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(nullable: false),
                ClaimType = table.Column<string>(nullable: true),
                ClaimValue = table.Column<string>(nullable: true)
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
                LoginProvider = table.Column<string>(nullable: false),
                ProviderKey = table.Column<string>(nullable: false),
                ProviderDisplayName = table.Column<string>(nullable: true),
                UserId = table.Column<string>(nullable: false)
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
                UserId = table.Column<string>(nullable: false),
                RoleId = table.Column<string>(nullable: false)
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
                UserId = table.Column<string>(nullable: false),
                LoginProvider = table.Column<string>(nullable: false),
                Name = table.Column<string>(nullable: false),
                Value = table.Column<string>(nullable: true)
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
            name: "IX_AspNetUsers_ApplicationRoleId",
            table: "AspNetUsers",
            column: "ApplicationRoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Mantle_BlogPosts_CategoryId",
            table: "Mantle_BlogPosts",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Mantle_BlogPostTags_TagId",
            table: "Mantle_BlogPostTags",
            column: "TagId");

        migrationBuilder.CreateIndex(
            name: "IX_Mantle_Common_Regions_ParentId",
            table: "Mantle_Common_Regions",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_Mantle_MessageTemplateVersions_MessageTemplateId",
            table: "Mantle_MessageTemplateVersions",
            column: "MessageTemplateId");

        migrationBuilder.CreateIndex(
            name: "IX_Mantle_PageVersions_PageId",
            table: "Mantle_PageVersions",
            column: "PageId");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_RoleId",
            table: "RolePermissions",
            column: "RoleId");
    }

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
            name: "Mantle_BlogPostTags");

        migrationBuilder.DropTable(
            name: "Mantle_Common_Regions");

        migrationBuilder.DropTable(
            name: "Mantle_Common_RegionSettings");

        migrationBuilder.DropTable(
            name: "Mantle_ContentBlocks");

        migrationBuilder.DropTable(
            name: "Mantle_EntityTypeContentBlocks");

        migrationBuilder.DropTable(
            name: "Mantle_GenericAttributes");

        migrationBuilder.DropTable(
            name: "Mantle_Languages");

        migrationBuilder.DropTable(
            name: "Mantle_LocalizableProperties");

        migrationBuilder.DropTable(
            name: "Mantle_LocalizableStrings");

        migrationBuilder.DropTable(
            name: "Mantle_Log");

        migrationBuilder.DropTable(
            name: "Mantle_MenuItems");

        migrationBuilder.DropTable(
            name: "Mantle_Menus");

        migrationBuilder.DropTable(
            name: "Mantle_MessageTemplateVersions");

        migrationBuilder.DropTable(
            name: "Mantle_PageTypes");

        migrationBuilder.DropTable(
            name: "Mantle_PageVersions");

        migrationBuilder.DropTable(
            name: "Mantle_QueuedEmails");

        migrationBuilder.DropTable(
            name: "Mantle_ScheduledTasks");

        migrationBuilder.DropTable(
            name: "Mantle_Settings");

        migrationBuilder.DropTable(
            name: "Mantle_SitemapConfig");

        migrationBuilder.DropTable(
            name: "Mantle_Tenants");

        migrationBuilder.DropTable(
            name: "Mantle_Zones");

        migrationBuilder.DropTable(
            name: "RolePermissions");

        migrationBuilder.DropTable(
            name: "UserProfiles");

        migrationBuilder.DropTable(
            name: "AspNetUsers");

        migrationBuilder.DropTable(
            name: "Mantle_BlogPosts");

        migrationBuilder.DropTable(
            name: "Mantle_BlogTags");

        migrationBuilder.DropTable(
            name: "Mantle_MessageTemplates");

        migrationBuilder.DropTable(
            name: "Mantle_Pages");

        migrationBuilder.DropTable(
            name: "Permissions");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "Mantle_BlogCategories");
    }
}