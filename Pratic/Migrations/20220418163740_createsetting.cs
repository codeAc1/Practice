using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pratic.Migrations
{
    public partial class createsetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "setting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Logo = table.Column<string>(maxLength: 255, nullable: false),
                    Offer = table.Column<string>(maxLength: 255, nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    SupportPhone = table.Column<string>(maxLength: 255, nullable: false),
                    Phone = table.Column<string>(maxLength: 255, nullable: false),
                    Address = table.Column<string>(maxLength: 255, nullable: false),
                    WorkDay = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_setting", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "setting");
        }
    }
}
