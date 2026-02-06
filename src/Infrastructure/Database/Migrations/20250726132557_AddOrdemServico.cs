using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdemServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ordens_servico",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_inicio_execucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_finalizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_entrega = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordens_servico", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "itens_incluidos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_estoque_original_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    tipo_item_incluido = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    preco = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ordem_servico_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itens_incluidos", x => x.id);
                    table.ForeignKey(
                        name: "FK_itens_incluidos_ordens_servico_ordem_servico_id",
                        column: x => x.ordem_servico_id,
                        principalTable: "ordens_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orcamentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    preco = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ordem_servico_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orcamentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_orcamentos_ordens_servico_ordem_servico_id",
                        column: x => x.ordem_servico_id,
                        principalTable: "ordens_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "servicos_incluidos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    servico_original_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    preco = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ordem_servico_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicos_incluidos", x => x.id);
                    table.ForeignKey(
                        name: "FK_servicos_incluidos_ordens_servico_ordem_servico_id",
                        column: x => x.ordem_servico_id,
                        principalTable: "ordens_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_itens_incluidos_ordem_servico_id",
                table: "itens_incluidos",
                column: "ordem_servico_id");

            migrationBuilder.CreateIndex(
                name: "IX_orcamentos_ordem_servico_id",
                table: "orcamentos",
                column: "ordem_servico_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_servicos_incluidos_ordem_servico_id",
                table: "servicos_incluidos",
                column: "ordem_servico_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "itens_incluidos");

            migrationBuilder.DropTable(
                name: "orcamentos");

            migrationBuilder.DropTable(
                name: "servicos_incluidos");

            migrationBuilder.DropTable(
                name: "ordens_servico");
        }
    }
}
