using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YYBagProgram.Migrations
{
    public partial class modifiedtablesechma : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "iBagsId",
                table: "OrderDetail",
                type: "varchar(8)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ProductColor",
                columns: table => new
                {
                    strBagsId = table.Column<string>(type: "varchar(8)", nullable: false),
                    strColor = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    strID = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColor", x => new { x.strColor, x.strBagsId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductColor");

            migrationBuilder.AlterColumn<int>(
                name: "iBagsId",
                table: "OrderDetail",
                type: "varchar(8)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "int");
        }
    }
}
