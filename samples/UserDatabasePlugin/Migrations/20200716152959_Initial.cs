using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace UserDatabasePlugin.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDatabasePlugin_Users",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 36, nullable: false),
                    Type = table.Column<string>(maxLength: 20, nullable: true)
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
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(maxLength: 32, nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDatabasePlugin_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDatabasePlugin_UserActivities_UserDatabasePlugin_Users_U~",
                        column: x => x.UserId,
                        principalTable: "UserDatabasePlugin_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
