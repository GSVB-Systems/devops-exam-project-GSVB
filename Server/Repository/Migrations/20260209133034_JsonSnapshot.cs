using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevOpsAppRepo.Migrations
{
    /// <inheritdoc />
    public partial class JsonSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserEggSnapshots",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    EiUserId = table.Column<string>(type: "text", nullable: true),
                    BoostsUsed = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    LastFetchedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RawJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEggSnapshots", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserEggSnapshots_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEggSnapshots");
        }
    }
}
