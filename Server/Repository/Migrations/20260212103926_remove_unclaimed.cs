using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevOpsAppRepo.Migrations
{
    /// <inheritdoc />
    public partial class remove_unclaimed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnclaimedGoldenEggs",
                table: "UserEggSnapshots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnclaimedGoldenEggs",
                table: "UserEggSnapshots",
                type: "numeric(20,0)",
                nullable: true);
        }
    }
}
