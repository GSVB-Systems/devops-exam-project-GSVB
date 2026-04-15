using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevOpsAppRepo.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EiUserIdHash",
                table: "UserEggSnapshots",
                newName: "UserName");

            migrationBuilder.AddColumn<double>(
                name: "Cer",
                table: "UserEggSnapshots",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CraftingXp",
                table: "UserEggSnapshots",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Eb",
                table: "UserEggSnapshots",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EggsOfProphecy",
                table: "UserEggSnapshots",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EiUserId",
                table: "UserEggSnapshots",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GoldenEggsBalance",
                table: "UserEggSnapshots",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GoldenEggsEarned",
                table: "UserEggSnapshots",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GoldenEggsSpent",
                table: "UserEggSnapshots",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Jer",
                table: "UserEggSnapshots",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Mer",
                table: "UserEggSnapshots",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SoulEggs",
                table: "UserEggSnapshots",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TruthEggs",
                table: "UserEggSnapshots",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnclaimedGoldenEggs",
                table: "UserEggSnapshots",
                type: "numeric(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cer",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "CraftingXp",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "Eb",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "EggsOfProphecy",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "EiUserId",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "GoldenEggsBalance",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "GoldenEggsEarned",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "GoldenEggsSpent",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "Jer",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "Mer",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "SoulEggs",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "TruthEggs",
                table: "UserEggSnapshots");

            migrationBuilder.DropColumn(
                name: "UnclaimedGoldenEggs",
                table: "UserEggSnapshots");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserEggSnapshots",
                newName: "EiUserIdHash");
        }
    }
}
