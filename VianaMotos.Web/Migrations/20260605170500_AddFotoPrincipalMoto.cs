using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VianaMotos.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoPrincipalMoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoPrincipal",
                table: "Motos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoPrincipal",
                table: "Motos");
        }
    }
}
