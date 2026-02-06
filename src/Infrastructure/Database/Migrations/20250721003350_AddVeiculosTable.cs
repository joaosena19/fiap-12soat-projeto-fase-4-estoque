using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVeiculosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "veiculos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    placa = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    modelo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    marca = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ano = table.Column<int>(type: "integer", nullable: false),
                    tipo_veiculo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_veiculos", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "veiculos");
        }
    }
}
