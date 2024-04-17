using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YYBagProgram.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    strMemberPhone = table.Column<string>(type: "varchar(15)", nullable: false),
                    strMemberId = table.Column<string>(type: "varchar(10)", nullable: false),
                    strMemberPassWord = table.Column<string>(type: "varchar(max)", nullable: false),
                    strMemberName = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    strMemberEmail = table.Column<string>(type: "varchar(30)", nullable: false),
                    dateBirthday = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.strMemberPhone);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    strOrderGuid = table.Column<string>(type: "varchar(20)", nullable: false),
                    iMemberId = table.Column<int>(type: "int", nullable: false),
                    strReceiver = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    strReceiverPhone = table.Column<string>(type: "varchar(15)", nullable: false),
                    strReceiverAddress = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.strOrderGuid);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    strOrderGuid = table.Column<string>(type: "varchar(20)", nullable: false),
                    strBagsId = table.Column<string>(type: "varchar(8)", nullable: false),
                    iFinalPrice = table.Column<int>(type: "int", nullable: false),
                    iQuantity = table.Column<int>(type: "int", nullable: false),
                    iOrderStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.strOrderGuid);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    strBagsId = table.Column<string>(type: "varchar(8)", nullable: false),
                    strBagsName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    BagType = table.Column<int>(type: "int", nullable: false),
                    strDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dLength = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    dWidth = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    dHigh = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    dWeight = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    strMaterial = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    dateLastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.strBagsId);
                });

            migrationBuilder.CreateTable(
                name: "ProductsColorDetail",
                columns: table => new
                {
                    strColor = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ProductStatus = table.Column<int>(type: "int", nullable: false),
                    strID = table.Column<string>(type: "varchar(20)", nullable: false),
                    iTotal = table.Column<int>(type: "int", nullable: false),
                    iRemain = table.Column<int>(type: "int", nullable: false),
                    iPrice = table.Column<int>(type: "int", nullable: false),
                    strPictureURL = table.Column<string>(type: "varchar(max)", nullable: false),
                    iDeliveryDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsColorDetail", x => new { x.strColor, x.ProductStatus });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductsColorDetail");
        }
    }
}
