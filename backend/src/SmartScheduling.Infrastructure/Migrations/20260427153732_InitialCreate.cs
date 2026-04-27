using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartScheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    estabelecimento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "conversas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    telefone_cliente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estabelecimento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    agendamento_pendente_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estabelecimentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    whatsapp_phone_number_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    proprietario_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estabelecimentos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mensagens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conteudo = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    veio_do_cliente = table.Column<bool>(type: "boolean", nullable: false),
                    texto_transcrito = table.Column<string>(type: "text", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensagens", x => x.id);
                    table.ForeignKey(
                        name: "FK_mensagens_conversas_conversa_id",
                        column: x => x.conversa_id,
                        principalTable: "conversas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bloqueios_estabelecimento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    estabelecimento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bloqueios_estabelecimento", x => x.id);
                    table.ForeignKey(
                        name: "FK_bloqueios_estabelecimento_estabelecimentos_estabelecimento_~",
                        column: x => x.estabelecimento_id,
                        principalTable: "estabelecimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "funcionarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    estabelecimento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funcionarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_funcionarios_estabelecimentos_estabelecimento_id",
                        column: x => x.estabelecimento_id,
                        principalTable: "estabelecimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "horarios_estabelecimento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    estabelecimento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dia_semana = table.Column<int>(type: "integer", nullable: false),
                    hora_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    hora_fim = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horarios_estabelecimento", x => x.id);
                    table.ForeignKey(
                        name: "FK_horarios_estabelecimento_estabelecimentos_estabelecimento_id",
                        column: x => x.estabelecimento_id,
                        principalTable: "estabelecimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "servicos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    duracao_minutos = table.Column<int>(type: "integer", nullable: false),
                    preco = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    estabelecimento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicos", x => x.id);
                    table.ForeignKey(
                        name: "FK_servicos_estabelecimentos_estabelecimento_id",
                        column: x => x.estabelecimento_id,
                        principalTable: "estabelecimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "horarios_trabalho",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dia_semana = table.Column<int>(type: "integer", nullable: false),
                    hora_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    hora_fim = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horarios_trabalho", x => x.id);
                    table.ForeignKey(
                        name: "FK_horarios_trabalho_funcionarios_funcionario_id",
                        column: x => x.funcionario_id,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agendamentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    servico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estabelecimento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inicio_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fim_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    motivo_cancelamento = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendamentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_agendamentos_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_agendamentos_funcionarios_funcionario_id",
                        column: x => x.funcionario_id,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_agendamentos_servicos_servico_id",
                        column: x => x.servico_id,
                        principalTable: "servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "funcionario_servicos",
                columns: table => new
                {
                    FuncionarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicosId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funcionario_servicos", x => new { x.FuncionarioId, x.ServicosId });
                    table.ForeignKey(
                        name: "FK_funcionario_servicos_funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_funcionario_servicos_servicos_ServicosId",
                        column: x => x.ServicosId,
                        principalTable: "servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecurringSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    DaysOfWeek = table.Column<string>(type: "varchar(20)", nullable: false),
                    DayOfMonth = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    StartsOn = table.Column<DateOnly>(type: "date", nullable: false),
                    EndsOn = table.Column<DateOnly>(type: "date", nullable: true),
                    MaxOccurrences = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringSchedules_clientes_ClientId",
                        column: x => x.ClientId,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringSchedules_funcionarios_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringSchedules_servicos_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agendamentos_cliente_id",
                table: "agendamentos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_agendamentos_estabelecimento_id",
                table: "agendamentos",
                column: "estabelecimento_id");

            migrationBuilder.CreateIndex(
                name: "ix_agendamentos_funcionario_id",
                table: "agendamentos",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_agendamentos_funcionario_status",
                table: "agendamentos",
                columns: new[] { "funcionario_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_servico_id",
                table: "agendamentos",
                column: "servico_id");

            migrationBuilder.CreateIndex(
                name: "ix_bloqueios_estabelecimento_estabelecimento_periodo",
                table: "bloqueios_estabelecimento",
                columns: new[] { "estabelecimento_id", "data_inicio", "data_fim" });

            migrationBuilder.CreateIndex(
                name: "ix_clientes_estabelecimento_email",
                table: "clientes",
                columns: new[] { "estabelecimento_id", "email" });

            migrationBuilder.CreateIndex(
                name: "ix_clientes_estabelecimento_id",
                table: "clientes",
                column: "estabelecimento_id");

            migrationBuilder.CreateIndex(
                name: "ix_conversas_estabelecimento_id",
                table: "conversas",
                column: "estabelecimento_id");

            migrationBuilder.CreateIndex(
                name: "ix_conversas_status",
                table: "conversas",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_estabelecimentos_whatsapp_phone_number_id",
                table: "estabelecimentos",
                column: "whatsapp_phone_number_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_funcionario_servicos_ServicosId",
                table: "funcionario_servicos",
                column: "ServicosId");

            migrationBuilder.CreateIndex(
                name: "ix_funcionarios_estabelecimento_email",
                table: "funcionarios",
                columns: new[] { "estabelecimento_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_funcionarios_estabelecimento_id",
                table: "funcionarios",
                column: "estabelecimento_id");

            migrationBuilder.CreateIndex(
                name: "ix_horarios_estabelecimento_estabelecimento_dia",
                table: "horarios_estabelecimento",
                columns: new[] { "estabelecimento_id", "dia_semana" });

            migrationBuilder.CreateIndex(
                name: "IX_horarios_trabalho_funcionario_id",
                table: "horarios_trabalho",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_mensagens_conversa_id",
                table: "mensagens",
                column: "conversa_id");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringSchedules_ClientId",
                table: "RecurringSchedules",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringSchedules_EmployeeId",
                table: "RecurringSchedules",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringSchedules_EstablishmentId_IsActive",
                table: "RecurringSchedules",
                columns: new[] { "EstablishmentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringSchedules_ServiceId",
                table: "RecurringSchedules",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "ix_servicos_estabelecimento_id",
                table: "servicos",
                column: "estabelecimento_id");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agendamentos");

            migrationBuilder.DropTable(
                name: "bloqueios_estabelecimento");

            migrationBuilder.DropTable(
                name: "funcionario_servicos");

            migrationBuilder.DropTable(
                name: "horarios_estabelecimento");

            migrationBuilder.DropTable(
                name: "horarios_trabalho");

            migrationBuilder.DropTable(
                name: "mensagens");

            migrationBuilder.DropTable(
                name: "RecurringSchedules");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "conversas");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "funcionarios");

            migrationBuilder.DropTable(
                name: "servicos");

            migrationBuilder.DropTable(
                name: "estabelecimentos");
        }
    }
}
