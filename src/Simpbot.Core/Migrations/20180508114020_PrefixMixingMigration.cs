using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Simpbot.Core.Migrations
{
    public partial class PrefixMixingMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrefixDefaults");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrefixDefaults",
                columns: table => new
                {
                    PrefixId = table.Column<char>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PrefixSymbol = table.Column<char>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrefixDefaults", x => x.PrefixId);
                });
        }
    }
}
