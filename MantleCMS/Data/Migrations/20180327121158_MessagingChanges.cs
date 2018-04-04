using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MantleCMS.Data.Migrations
{
    public partial class MessagingChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "RolePermissions",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.DropTable(name: "Mantle_MessageTemplates");

            migrationBuilder.CreateTable(
                name: "Mantle_MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_MessageTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_MessageTemplateVersions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CultureCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    Data = table.Column<string>(nullable: true),
                    DateCreatedUtc = table.Column<DateTime>(nullable: false),
                    DateModifiedUtc = table.Column<DateTime>(nullable: false),
                    MessageTemplateId = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(maxLength: 255, nullable: false)
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
                name: "Mantle_Plugins_Forums_Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Plugins_Forums_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_Plugins_Forums_PrivateMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    FromUserId = table.Column<string>(maxLength: 128, nullable: false),
                    IsDeletedByAuthor = table.Column<bool>(nullable: false),
                    IsDeletedByRecipient = table.Column<bool>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    Subject = table.Column<string>(maxLength: 512, nullable: false),
                    Text = table.Column<string>(nullable: false),
                    ToUserId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Plugins_Forums_PrivateMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_Plugins_Forums_Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ForumId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Plugins_Forums_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_Plugins_Forums_Forums",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    ForumGroupId = table.Column<int>(nullable: false),
                    LastPostId = table.Column<int>(nullable: false),
                    LastPostTime = table.Column<DateTime>(nullable: true),
                    LastPostUserId = table.Column<string>(maxLength: 128, nullable: true),
                    LastTopicId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    NumPosts = table.Column<int>(nullable: false),
                    NumTopics = table.Column<int>(nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Plugins_Forums_Forums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mantle_Plugins_Forums_Forums_Mantle_Plugins_Forums_Groups_ForumGroupId",
                        column: x => x.ForumGroupId,
                        principalTable: "Mantle_Plugins_Forums_Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_Plugins_Forums_Topics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ForumId = table.Column<int>(nullable: false),
                    LastPostId = table.Column<int>(nullable: false),
                    LastPostTime = table.Column<DateTime>(nullable: true),
                    LastPostUserId = table.Column<string>(maxLength: 128, nullable: true),
                    NumPosts = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(maxLength: 512, nullable: false),
                    TopicType = table.Column<byte>(nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: false),
                    Views = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Plugins_Forums_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mantle_Plugins_Forums_Topics_Mantle_Plugins_Forums_Forums_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Mantle_Plugins_Forums_Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mantle_Plugins_Forums_Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    IPAddress = table.Column<string>(unicode: false, maxLength: 45, nullable: true),
                    Text = table.Column<string>(nullable: false),
                    TopicId = table.Column<int>(nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Plugins_Forums_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mantle_Plugins_Forums_Posts_Mantle_Plugins_Forums_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Mantle_Plugins_Forums_Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mantle_MessageTemplateVersions_MessageTemplateId",
                table: "Mantle_MessageTemplateVersions",
                column: "MessageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Mantle_Plugins_Forums_Forums_ForumGroupId",
                table: "Mantle_Plugins_Forums_Forums",
                column: "ForumGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Mantle_Plugins_Forums_Posts_TopicId",
                table: "Mantle_Plugins_Forums_Posts",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Mantle_Plugins_Forums_Topics_ForumId",
                table: "Mantle_Plugins_Forums_Topics",
                column: "ForumId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mantle_MessageTemplateVersions");

            migrationBuilder.DropTable(
                name: "Mantle_Plugins_Forums_Posts");

            migrationBuilder.DropTable(
                name: "Mantle_Plugins_Forums_PrivateMessages");

            migrationBuilder.DropTable(
                name: "Mantle_Plugins_Forums_Subscriptions");

            migrationBuilder.DropTable(
                name: "Mantle_Plugins_Forums_Topics");

            migrationBuilder.DropTable(
                name: "Mantle_Plugins_Forums_Forums");

            migrationBuilder.DropTable(
                name: "Mantle_Plugins_Forums_Groups");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "RolePermissions",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.DropTable(name: "Mantle_MessageTemplates");

            migrationBuilder.CreateTable(
                name: "Mantle_MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_MessageTemplates", x => x.Id);
                });
        }
    }
}