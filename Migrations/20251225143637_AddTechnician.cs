using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnicianAgenda.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PagoATecnicoRealizado",
                table: "Works",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PorPagarATecnico",
                table: "Works",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianId",
                table: "Works",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Technician",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nacionalidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutOPasaporte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FotografiaPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<int>(type: "int", nullable: false),
                    Comuna = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroTelefonico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroTelefonicoAlternativo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatenteVehiculo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certificaciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technician", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Works_TechnicianId",
                table: "Works",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Technician_TechnicianId",
                table: "Works",
                column: "TechnicianId",
                principalTable: "Technician",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_Technician_TechnicianId",
                table: "Works");

            migrationBuilder.DropTable(
                name: "Technician");

            migrationBuilder.DropIndex(
                name: "IX_Works_TechnicianId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "PagoATecnicoRealizado",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "PorPagarATecnico",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "Works");
        }
    }
}
