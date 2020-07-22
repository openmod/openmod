using Microsoft.EntityFrameworkCore.Migrations;

namespace UserDatabasePlugin.Migrations
{
    public partial class UserActivityAddUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UserDatabasePlugin_UserActivities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UserDatabasePlugin_UserActivities");
        }
    }
}
