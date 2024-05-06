using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YYBagProgram.Migrations
{
    public partial class addisgoogleaccountonmembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isReview",
                table: "Member",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bool");

            migrationBuilder.AddColumn<bool>(
                name: "isGoogleAccount",
                table: "Member",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isGoogleAccount",
                table: "Member");

            migrationBuilder.AlterColumn<bool>(
                name: "isReview",
                table: "Member",
                type: "bool",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
