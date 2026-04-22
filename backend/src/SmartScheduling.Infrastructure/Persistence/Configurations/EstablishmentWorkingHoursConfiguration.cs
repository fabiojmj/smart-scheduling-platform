using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class EstablishmentWorkingHoursConfiguration : IEntityTypeConfiguration<EstablishmentWorkingHours>
{
    public void Configure(EntityTypeBuilder<EstablishmentWorkingHours> builder)
    {
        builder.ToTable("horarios_estabelecimento");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EstabelecimentoId).HasColumnName("estabelecimento_id").IsRequired();
        builder.Property(e => e.DiaSemana).HasColumnName("dia_semana").IsRequired();
        builder.Property(e => e.HoraInicio).HasColumnName("hora_inicio").IsRequired();
        builder.Property(e => e.HoraFim).HasColumnName("hora_fim").IsRequired();
        builder.Property(e => e.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasOne(e => e.Estabelecimento)
            .WithMany()
            .HasForeignKey(e => e.EstabelecimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.EstabelecimentoId, e.DiaSemana })
            .HasDatabaseName("ix_horarios_estabelecimento_estabelecimento_dia");

        builder.Ignore(e => e.DomainEvents);
    }
}
