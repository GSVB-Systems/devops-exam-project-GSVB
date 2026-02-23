using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevOpsAppRepo.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernamesAndEggAccountId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEggSnapshots",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "DiscordUsername",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EiUserId",
                table: "UserEggSnapshots",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "UserEggSnapshots",
                type: "text",
                nullable: false,
                defaultValueSql: "gen_random_uuid()::text");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "UserEggSnapshots",
                type: "text",
                nullable: false,
                defaultValue: "Alt");

            migrationBuilder.Sql("UPDATE \"UserEggSnapshots\" SET \"EiUserId\" = '' WHERE \"EiUserId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"UserEggSnapshots\" SET \"Id\" = gen_random_uuid()::text WHERE \"Id\" = '' OR \"Id\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"UserEggSnapshots\" SET \"Status\" = 'Main' WHERE \"Status\" = '' OR \"Status\" IS NULL;");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEggSnapshots",
                table: "UserEggSnapshots",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserEggSnapshots_UserId_EiUserId",
                table: "UserEggSnapshots",
                columns: new[] { "UserId", "EiUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEggSnapshots",
                table: "UserEggSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_UserEggSnapshots_UserId_EiUserId",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "DiscordUsername",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserEggSnapshots");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "EiUserId",
                table: "UserEggSnapshots",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEggSnapshots",
                table: "UserEggSnapshots",
                column: "UserId");
        }
    }
}
