using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YYBagProgram.Migrations
{
    public partial class modifymonthlytocarouselsetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MonthlyHots",
                table: "MonthlyHots");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "MonthlyHots");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "MonthlyHots");

            migrationBuilder.DropColumn(
                name: "img",
                table: "MonthlyHots");

            migrationBuilder.AlterColumn<string>(
                name: "Images",
                table: "ProductsColorDetail",
                type: "varchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imgurl",
                table: "MonthlyHots",
                type: "varchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonthlyHots",
                table: "MonthlyHots",
                column: "strBagsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MonthlyHots",
                table: "MonthlyHots");

            migrationBuilder.DropColumn(
                name: "imgurl",
                table: "MonthlyHots");

            migrationBuilder.AlterColumn<string>(
                name: "Images",
                table: "ProductsColorDetail",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "MonthlyHots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "MonthlyHots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "img",
                table: "MonthlyHots",
                type: "varchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonthlyHots",
                table: "MonthlyHots",
                columns: new[] { "Year", "Month", "strBagsId" });
        }
    }
}
