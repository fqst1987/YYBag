using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YYBagProgram.Migrations
{
    public partial class addmonthlyhot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
    name: "PK_MonthlyHots",
    table: "MonthlyHots");

            migrationBuilder.AlterColumn<int>(
                name: "Month",
                table: "MonthlyHots",
                type: "int",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "MonthlyHots",
                type: "int",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "img",
                table: "MonthlyHots",
                type: "varchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonthlyHots",
                table: "MonthlyHots",
                columns: new[] { "Year", "Month", "strBagsId" });

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

            migrationBuilder.DropPrimaryKey(
    name: "PK_MonthlyHots",
    table: "MonthlyHots");

            migrationBuilder.DropColumn(
                name: "img",
                table: "MonthlyHots");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Month",
                table: "MonthlyHots",
                type: "date",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Year",
                table: "MonthlyHots",
                type: "date",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonthlyHots",
                table: "MonthlyHots",
                columns: new[] { "Year", "Month" });
        }
    }
}
