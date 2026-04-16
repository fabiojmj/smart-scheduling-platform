using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class ConversaConfiguration : IEntityTypeConfiguration<Conversa>
{
    public void Configure(EntityTypeBuilder<Conversa> builder)
    {
        builder.ToTable("conversas");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.OwnsOne(c => c.TelefoneCliente, tel =>
        {
            tel.Property(t => t.Value)
                .HasColumnName("telefone_cliente")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(c => c.EstabelecimentoId).HasColumnName("estabelecimento_id").IsRequired();
        builder.Property(c => c.Status).HasColumnName("status").IsRequired();
        builder.Property(c => c.AgendamentoPendenteId).HasColumnName("agendamento_pendente_id");
        builder.Property(c => c.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(c => c.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasMany(c => c.Mensagens)
            .WithOne()
            .HasForeignKey(m => m.ConversaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.EstabelecimentoId).HasDatabaseName("ix_conversas_estabelecimento_id");
        builder.HasIndex(c => c.Status).HasDatabaseName("ix_conversas_status");

        builder.Ignore(c => c.DomainEvents);
    }
}
