using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnicianAgenda.Migrations
{
    /// <inheritdoc />
    public partial class AddJobCategoriesSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobType",
                table: "Works",
                newName: "Detalles");

            migrationBuilder.AddColumn<int>(
                name: "JobCategoryId",
                table: "Works",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobSubcategoryId",
                table: "Works",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Comuna",
                table: "Directions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Referencia",
                table: "Directions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Directions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Apellidos",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorreoElectronico",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "JobCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobSubcategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    JobCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSubcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSubcategories_JobCategories_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Works_JobCategoryId",
                table: "Works",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Works_JobSubcategoryId",
                table: "Works",
                column: "JobSubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSubcategories_JobCategoryId",
                table: "JobSubcategories",
                column: "JobCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_JobCategories_JobCategoryId",
                table: "Works",
                column: "JobCategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_JobSubcategories_JobSubcategoryId",
                table: "Works",
                column: "JobSubcategoryId",
                principalTable: "JobSubcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_JobCategories_JobCategoryId",
                table: "Works");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_JobSubcategories_JobSubcategoryId",
                table: "Works");

            migrationBuilder.DropTable(
                name: "JobSubcategories");

            migrationBuilder.DropTable(
                name: "JobCategories");

            migrationBuilder.DropIndex(
                name: "IX_Works_JobCategoryId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_JobSubcategoryId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "JobCategoryId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "JobSubcategoryId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "Comuna",
                table: "Directions");

            migrationBuilder.DropColumn(
                name: "Referencia",
                table: "Directions");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Directions");

            migrationBuilder.DropColumn(
                name: "Apellidos",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CorreoElectronico",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "Detalles",
                table: "Works",
                newName: "JobType");
        }
    }
}
