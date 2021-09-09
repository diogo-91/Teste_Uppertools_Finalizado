using Microsoft.EntityFrameworkCore.Migrations;

namespace CadastroEmpresa.Migrations
{
    public partial class ApagadosCampos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Empresas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
