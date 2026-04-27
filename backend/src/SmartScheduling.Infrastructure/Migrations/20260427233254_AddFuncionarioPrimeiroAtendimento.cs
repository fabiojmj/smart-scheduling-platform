using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartScheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFuncionarioPrimeiroAtendimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "funcionario_primeiro_atendimento_id",
                table: "estabelecimentos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_estabelecimentos_funcionario_primeiro_atendimento_id",
                table: "estabelecimentos",
                column: "funcionario_primeiro_atendimento_id");

            migrationBuilder.AddForeignKey(
                name: "FK_estabelecimentos_funcionarios_funcionario_primeiro_atendime~",
                table: "estabelecimentos",
                column: "funcionario_primeiro_atendimento_id",
                principalTable: "funcionarios",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estabelecimentos_funcionarios_funcionario_primeiro_atendime~",
                table: "estabelecimentos");

            migrationBuilder.DropIndex(
                name: "IX_estabelecimentos_funcionario_primeiro_atendimento_id",
                table: "estabelecimentos");

            migrationBuilder.DropColumn(
                name: "funcionario_primeiro_atendimento_id",
                table: "estabelecimentos");
        }
    }
}
