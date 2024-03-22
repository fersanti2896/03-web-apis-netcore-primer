using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoresAPI.Migrations
{
    /// <inheritdoc />
    public partial class Peticiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PeticionesAPI",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    FechaPeticion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeticionesAPI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeticionesAPI_LlavesAPI_LlaveId",
                        column: x => x.LlaveId,
                        principalTable: "LlavesAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeticionesAPI_LlaveId",
                table: "PeticionesAPI",
                column: "LlaveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeticionesAPI");
        }
    }
}
