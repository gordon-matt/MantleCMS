using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MantleCMS.Data.Migrations
{
    public partial class UpgradeNetCore2_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserProfiles",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "RolePermissions",
                type: "nvarchar(450)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationRoleId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Mantle_Common_Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountryCode = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: true),
                    HasStates = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Order = table.Column<short>(type: "smallint", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    RegionType = table.Column<byte>(type: "tinyint", nullable: false),
                    StateCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    SettingsId = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantle_Common_RegionSettings", x => x.Id);
                });

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
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Mantle_Common_Regions_ParentId",
                table: "Mantle_Common_Regions",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_ApplicationRoleId",
                table: "AspNetUsers",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_ApplicationRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Mantle_Common_Regions");

            migrationBuilder.DropTable(
                name: "Mantle_Common_RegionSettings");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ApplicationRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserProfiles",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "RolePermissions",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldUnicode: false,
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);
        }
    }
}