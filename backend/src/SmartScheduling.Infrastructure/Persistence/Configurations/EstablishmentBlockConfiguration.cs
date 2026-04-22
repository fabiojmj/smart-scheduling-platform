using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class EstablishmentBlockConfiguration : IEntityTypeConfiguration<EstablishmentBlock>
{
    public void Configure(EntityTypeBuilder<EstablishmentBlock> builder)
    {
        builder.ToTable("bloqueios_estabelecimento");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EstabelecimentoId).HasColumnName("estabelecimento_id").IsRequired();
        builder.Property(e => e.DataInicio).HasColumnName("data_inicio").IsRequired();
        builder.Property(e => e.DataFim).HasColumnName("data_fim").IsRequired();
        builder.Property(e => e.Motivo).HasColumnName("motivo").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasOne(e => e.Estabelecimento)
            .WithMany()
            .HasForeignKey(e => e.EstabelecimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.EstabelecimentoId, e.DataInicio, e.DataFim })
            .HasDatabaseName("ix_bloqueios_estabelecimento_estabelecimento_periodo");

        builder.Ignore(e => e.DomainEvents);
    }
}
