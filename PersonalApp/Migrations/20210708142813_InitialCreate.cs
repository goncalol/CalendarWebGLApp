using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Todos",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
            //        AllDay = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Todos", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Todos");
        }
    }
}
