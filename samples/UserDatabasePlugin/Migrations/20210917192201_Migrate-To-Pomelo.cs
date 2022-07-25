using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserDatabasePlugin.Migrations
{
    public partial class MigrateToPomelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDatabasePlugin_Users",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 36, nullable: false),
                    Type = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDatabasePlugin_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDatabasePlugin_UserActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    Type = table.Column<string>(maxLength: 32, nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDatabasePlugin_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDatabasePlugin_UserActivities_UserDatabasePlugin_Users_U~",
                        column: x => x.UserId,
                        principalTable: "UserDatabasePlugin_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDatabasePlugin_UserActivities_UserId",
                table: "UserDatabasePlugin_UserActivities",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDatabasePlugin_UserActivities");

            migrationBuilder.DropTable(
                name: "UserDatabasePlugin_Users");
        }
    }
}
