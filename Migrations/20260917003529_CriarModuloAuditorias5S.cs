using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace App5s.Migrations
{
    /// <inheritdoc />
    public partial class CriarModuloAuditorias5S : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "itens_checklist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Senso = table.Column<int>(type: "integer", nullable: false),
                    Pergunta = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    GuiaAvaliacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Peso = table.Column<int>(type: "integer", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itens_checklist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "setores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ResponsavelId = table.Column<int>(type: "integer", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_setores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_setores_usuarios_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "auditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SetorId = table.Column<int>(type: "integer", nullable: false),
                    AuditorId = table.Column<int>(type: "integer", nullable: false),
                    DataRealizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PontuacaoGeral = table.Column<decimal>(type: "numeric", nullable: false),
                    ObservacoesGerais = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_auditorias_setores_SetorId",
                        column: x => x.SetorId,
                        principalTable: "setores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_auditorias_usuarios_AuditorId",
                        column: x => x.AuditorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "respostas_auditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuditoriaId = table.Column<int>(type: "integer", nullable: false),
                    ItemChecklistId = table.Column<int>(type: "integer", nullable: false),
                    Nota = table.Column<int>(type: "integer", nullable: false),
                    ObservacaoNaoConformidade = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EvidenciaFotoUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_respostas_auditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_respostas_auditoria_auditorias_AuditoriaId",
                        column: x => x.AuditoriaId,
                        principalTable: "auditorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_respostas_auditoria_itens_checklist_ItemChecklistId",
                        column: x => x.ItemChecklistId,
                        principalTable: "itens_checklist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auditorias_AuditorId",
                table: "auditorias",
                column: "AuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_auditorias_SetorId",
                table: "auditorias",
                column: "SetorId");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_auditoria_AuditoriaId",
                table: "respostas_auditoria",
                column: "AuditoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_auditoria_ItemChecklistId",
                table: "respostas_auditoria",
                column: "ItemChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_setores_ResponsavelId",
                table: "setores",
                column: "ResponsavelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "respostas_auditoria");

            migrationBuilder.DropTable(
                name: "auditorias");

            migrationBuilder.DropTable(
                name: "itens_checklist");

            migrationBuilder.DropTable(
                name: "setores");

        }
    }
}
