using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
{
    public void Configure(EntityTypeBuilder<Funcionario> builder)
    {
        builder.ToTable("funcionarios");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnName("id");

        builder.Property(f => f.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.Email)
            .HasColumnName("email")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.EstabelecimentoId).HasColumnName("estabelecimento_id").IsRequired();
        builder.Property(f => f.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(f => f.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(f => f.UpdatedAt).HasColumnName("atualizado_em");

        builder.OwnsMany(f => f.Horarios, h =>
        {
            h.ToTable("horarios_trabalho");
            h.WithOwner().HasForeignKey("funcionario_id");
            h.Property<int>("id").ValueGeneratedOnAdd();
            h.HasKey("id");
            h.Property(w => w.DayOfWeek).HasColumnName("dia_semana").IsRequired();
            h.Property(w => w.Start).HasColumnName("hora_inicio").IsRequired();
            h.Property(w => w.End).HasColumnName("hora_fim").IsRequired();
        });

        builder.HasMany(f => f.Servicos)
            .WithMany()
            .UsingEntity(j => j.ToTable("funcionario_servicos"));

        builder.HasIndex(f => f.EstabelecimentoId).HasDatabaseName("ix_funcionarios_estabelecimento_id");
        builder.HasIndex(f => new { f.EstabelecimentoId, f.Email })
            .IsUnique()
            .HasDatabaseName("ix_funcionarios_estabelecimento_email");

        builder.Ignore(f => f.DomainEvents);
    }
}
