using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.ToTable("servicos");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(1000);

        builder.Property(s => s.DuracaoMinutos).HasColumnName("duracao_minutos").IsRequired();

        builder.Property(s => s.Preco)
            .HasColumnName("preco")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.EstabelecimentoId).HasColumnName("estabelecimento_id").IsRequired();
        builder.Property(s => s.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(s => s.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(s => s.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasIndex(s => s.EstabelecimentoId).HasDatabaseName("ix_servicos_estabelecimento_id");

        builder.Ignore(s => s.DomainEvents);
    }
}
