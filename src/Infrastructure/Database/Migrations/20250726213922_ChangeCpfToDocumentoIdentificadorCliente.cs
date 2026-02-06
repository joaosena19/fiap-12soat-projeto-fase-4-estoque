using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCpfToDocumentoIdentificadorCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cpf",
                table: "clientes",
                newName: "documento_identificador");

            migrationBuilder.AlterColumn<string>(
                name: "documento_identificador",
                table: "clientes",
                type: "character varying(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11);

            migrationBuilder.AddColumn<string>(
                name: "tipo_documento_identificador",
                table: "clientes",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tipo_documento_identificador",
                table: "clientes");

            migrationBuilder.RenameColumn(
                name: "documento_identificador",
                table: "clientes",
                newName: "cpf");

            migrationBuilder.AlterColumn<string>(
                name: "cpf",
                table: "clientes",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(14)",
                oldMaxLength: 14);
        }
    }
}
