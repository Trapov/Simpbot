using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Simpbot.Core.Migrations
{
    public partial class QuoteMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    MessageId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    DateTime = table.Column<DateTimeOffset>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    UserId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => new { x.MessageId, x.GuildId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Muteds_UserId",
                table: "Muteds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_MessageId_GuildId_UserId",
                table: "Quotes",
                columns: new[] { "MessageId", "GuildId", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_Muteds_UserId",
                table: "Muteds");
        }
    }
}
