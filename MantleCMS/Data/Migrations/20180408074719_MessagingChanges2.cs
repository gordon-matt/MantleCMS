using Microsoft.EntityFrameworkCore.Migrations;

namespace MantleCMS.Data.Migrations
{
    public partial class MessagingChanges2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayCondition",
                table: "Mantle_ContentBlocks");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "RolePermissions",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "Editor",
                table: "Mantle_MessageTemplates",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Editor",
                table: "Mantle_MessageTemplates");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "RolePermissions",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 450);

            migrationBuilder.AddColumn<string>(
                name: "DisplayCondition",
                table: "Mantle_ContentBlocks",
                maxLength: 255,
                nullable: true);
        }
    }
}