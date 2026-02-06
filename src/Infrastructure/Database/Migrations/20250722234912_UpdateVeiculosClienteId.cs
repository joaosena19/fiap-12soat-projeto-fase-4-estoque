using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVeiculosClienteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "cliente_id",
                table: "veiculos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_cliente_id",
                table: "veiculos",
                column: "cliente_id");

            migrationBuilder.AddForeignKey(
                name: "FK_veiculos_clientes_cliente_id",
                table: "veiculos",
                column: "cliente_id",
                principalTable: "clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_veiculos_clientes_cliente_id",
                table: "veiculos");

            migrationBuilder.DropIndex(
                name: "IX_veiculos_cliente_id",
                table: "veiculos");

            migrationBuilder.DropColumn(
                name: "cliente_id",
                table: "veiculos");
        }
    }
}
