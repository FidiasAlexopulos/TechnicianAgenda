using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnicianAgenda.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianAndUpdateWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_Technician_TechnicianId",
                table: "Works");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Technician",
                table: "Technician");

            migrationBuilder.RenameTable(
                name: "Technician",
                newName: "Technicians");

            migrationBuilder.AlterColumn<string>(
                name: "CorreoElectronico",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RutOPasaporte",
                table: "Technicians",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CorreoElectronico",
                table: "Technicians",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Technicians",
                table: "Technicians",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CorreoElectronico",
                table: "Clients",
                column: "CorreoElectronico");

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_CorreoElectronico",
                table: "Technicians",
                column: "CorreoElectronico");

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_RutOPasaporte",
                table: "Technicians",
                column: "RutOPasaporte",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Technicians_TechnicianId",
                table: "Works",
                column: "TechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_Technicians_TechnicianId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Clients_CorreoElectronico",
                table: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Technicians",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_Technicians_CorreoElectronico",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_Technicians_RutOPasaporte",
                table: "Technicians");

            migrationBuilder.RenameTable(
                name: "Technicians",
                newName: "Technician");

            migrationBuilder.AlterColumn<string>(
                name: "CorreoElectronico",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RutOPasaporte",
                table: "Technician",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CorreoElectronico",
                table: "Technician",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Technician",
                table: "Technician",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Technician_TechnicianId",
                table: "Works",
                column: "TechnicianId",
                principalTable: "Technician",
                principalColumn: "Id");
        }
    }
}
