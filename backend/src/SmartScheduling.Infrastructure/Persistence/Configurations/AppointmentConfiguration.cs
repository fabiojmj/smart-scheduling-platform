using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("agendamentos");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");

        builder.OwnsOne(a => a.Horario, h =>
        {
            h.Property(t => t.Start).HasColumnName("inicio_em").IsRequired();
            h.Property(t => t.End).HasColumnName("fim_em").IsRequired();
        });

        builder.Property(a => a.ClienteId).HasColumnName("cliente_id").IsRequired();
        builder.Property(a => a.FuncionarioId).HasColumnName("funcionario_id").IsRequired();
        builder.Property(a => a.ServicoId).HasColumnName("servico_id").IsRequired();
        builder.Property(a => a.EstabelecimentoId).HasColumnName("estabelecimento_id").IsRequired();
        builder.Property(a => a.Status).HasColumnName("status").IsRequired();
        builder.Property(a => a.Observacoes).HasColumnName("observacoes").HasMaxLength(1000);
        builder.Property(a => a.MotivoCancelamento).HasColumnName("motivo_cancelamento").HasMaxLength(500);
        builder.Property(a => a.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(a => a.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasOne(a => a.Cliente)
            .WithMany(c => c.Agendamentos)
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Funcionario)
            .WithMany(f => f.Agendamentos)
            .HasForeignKey(a => a.FuncionarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Servico)
            .WithMany()
            .HasForeignKey(a => a.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.EstabelecimentoId).HasDatabaseName("ix_agendamentos_estabelecimento_id");
        builder.HasIndex(a => a.ClienteId).HasDatabaseName("ix_agendamentos_cliente_id");
        builder.HasIndex(a => a.FuncionarioId).HasDatabaseName("ix_agendamentos_funcionario_id");
        builder.HasIndex(a => new { a.FuncionarioId, a.Status }).HasDatabaseName("ix_agendamentos_funcionario_status");

        builder.Ignore(a => a.DomainEvents);
    }
}
