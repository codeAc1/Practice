using Microsoft.EntityFrameworkCore.Migrations;

namespace Pratic.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_setting",
                table: "setting");

            migrationBuilder.RenameTable(
                name: "setting",
                newName: "Settings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                table: "Settings",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                table: "Settings");

            migrationBuilder.RenameTable(
                name: "Settings",
                newName: "setting");

            migrationBuilder.AddPrimaryKey(
                name: "PK_setting",
                table: "setting",
                column: "Id");
        }
    }
}
