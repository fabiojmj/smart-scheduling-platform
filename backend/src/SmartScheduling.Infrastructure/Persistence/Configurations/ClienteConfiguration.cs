using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(c => c.Telefone, tel =>
        {
            tel.Property(t => t.Value)
                .HasColumnName("telefone")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(200);

        builder.Property(c => c.EstabelecimentoId).HasColumnName("estabelecimento_id").IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(c => c.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasIndex(c => c.EstabelecimentoId).HasDatabaseName("ix_clientes_estabelecimento_id");
        builder.HasIndex(c => new { c.EstabelecimentoId, c.Email }).HasDatabaseName("ix_clientes_estabelecimento_email");

        builder.Ignore(c => c.DomainEvents);
    }
}
