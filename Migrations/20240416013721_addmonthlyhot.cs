using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YYBagProgram.Migrations
{
    public partial class addmonthlyhot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "MonthlyHots",
                columns: table => new
                {
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    strBagsId = table.Column<string>(type: "varchar(8)", nullable: false),
                    img = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyHots", x => new { x.Year, x.Month, x.strBagsId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyHots");
        }
    }
}
