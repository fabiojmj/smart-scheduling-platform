using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class MensagemConfiguration : IEntityTypeConfiguration<Mensagem>
{
    public void Configure(EntityTypeBuilder<Mensagem> builder)
    {
        builder.ToTable("mensagens");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");

        builder.Property(m => m.ConversaId).HasColumnName("conversa_id").IsRequired();

        builder.Property(m => m.Conteudo)
            .HasColumnName("conteudo")
            .IsRequired();

        builder.Property(m => m.Tipo).HasColumnName("tipo").IsRequired();
        builder.Property(m => m.VeioDoCliente).HasColumnName("veio_do_cliente").IsRequired();
        builder.Property(m => m.TextoTranscrito).HasColumnName("texto_transcrito");
        builder.Property(m => m.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(m => m.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasIndex(m => m.ConversaId).HasDatabaseName("ix_mensagens_conversa_id");

        builder.Ignore(m => m.DomainEvents);
    }
}
