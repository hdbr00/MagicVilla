using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNumeroVillaTabla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumeroVillas",
                columns: table => new
                {
                    VillaNo = table.Column<int>(type: "int", nullable: false),
                    VillaId = table.Column<int>(type: "int", nullable: false),
                    DetalleEspecial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumeroVillas", x => x.VillaNo);
                    table.ForeignKey(
                        name: "FK_NumeroVillas_villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Fecha", "FechaActualizacion" },
                values: new object[] { new DateTime(2023, 10, 7, 0, 22, 26, 539, DateTimeKind.Local).AddTicks(6663), new DateTime(2023, 10, 7, 0, 22, 26, 539, DateTimeKind.Local).AddTicks(6673) });

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Fecha", "FechaActualizacion" },
                values: new object[] { new DateTime(2023, 10, 7, 0, 22, 26, 539, DateTimeKind.Local).AddTicks(6675), new DateTime(2023, 10, 7, 0, 22, 26, 539, DateTimeKind.Local).AddTicks(6676) });

            migrationBuilder.CreateIndex(
                name: "IX_NumeroVillas_VillaId",
                table: "NumeroVillas",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumeroVillas");

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Fecha", "FechaActualizacion" },
                values: new object[] { new DateTime(2023, 9, 29, 17, 15, 44, 211, DateTimeKind.Local).AddTicks(8855), new DateTime(2023, 9, 29, 17, 15, 44, 211, DateTimeKind.Local).AddTicks(8863) });

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Fecha", "FechaActualizacion" },
                values: new object[] { new DateTime(2023, 9, 29, 17, 15, 44, 211, DateTimeKind.Local).AddTicks(8865), new DateTime(2023, 9, 29, 17, 15, 44, 211, DateTimeKind.Local).AddTicks(8865) });
        }
    }
}
