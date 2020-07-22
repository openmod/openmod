using Microsoft.EntityFrameworkCore.Migrations;

namespace UserDatabasePlugin.Migrations
{
    public partial class AddMissingNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatabasePlugin_UserActivities_UserDatabasePlugin_Users_U~",
                table: "UserDatabasePlugin_UserActivities");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "UserDatabasePlugin_Users",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserDatabasePlugin_UserActivities",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatabasePlugin_UserActivities_UserDatabasePlugin_Users_U~",
                table: "UserDatabasePlugin_UserActivities",
                column: "UserId",
                principalTable: "UserDatabasePlugin_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatabasePlugin_UserActivities_UserDatabasePlugin_Users_U~",
                table: "UserDatabasePlugin_UserActivities");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "UserDatabasePlugin_Users",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserDatabasePlugin_UserActivities",
                type: "varchar(36)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatabasePlugin_UserActivities_UserDatabasePlugin_Users_U~",
                table: "UserDatabasePlugin_UserActivities",
                column: "UserId",
                principalTable: "UserDatabasePlugin_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
