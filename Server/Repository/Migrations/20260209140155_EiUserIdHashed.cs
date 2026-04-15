using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevOpsAppRepo.Migrations
{
    /// <inheritdoc />
    public partial class EiUserIdHashed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EiUserId",
                table: "UserEggSnapshots",
                newName: "EiUserIdHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EiUserIdHash",
                table: "UserEggSnapshots",
                newName: "EiUserId");
        }
    }
}
