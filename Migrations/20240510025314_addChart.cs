using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YYBagProgram.Migrations
{
    public partial class addChart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "strOrderGuid",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "strOrderGuid",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "iMemberId",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "strOrderId",
                table: "OrderDetail",
                type: "varchar(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "iTotal",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "strOrderId",
                table: "Order",
                type: "varchar(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "strMemberId",
                table: "Order",
                type: "varchar(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail",
                column: "strOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "strOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "strOrderId",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "iTotal",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "strOrderId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "strMemberId",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "strOrderGuid",
                table: "OrderDetail",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "strOrderGuid",
                table: "Order",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "iMemberId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail",
                column: "strOrderGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "strOrderGuid");
        }
    }
}
