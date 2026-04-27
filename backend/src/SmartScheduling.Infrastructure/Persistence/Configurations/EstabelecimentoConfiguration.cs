using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Infrastructure.Persistence.Configurations;

public class EstabelecimentoConfiguration : IEntityTypeConfiguration<Estabelecimento>
{
    public void Configure(EntityTypeBuilder<Estabelecimento> builder)
    {
        builder.ToTable("estabelecimentos");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.WhatsAppPhoneNumberId)
            .HasColumnName("whatsapp_phone_number_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.ProprietarioId)
            .HasColumnName("proprietario_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Ativo).HasColumnName("ativo").IsRequired();

        builder.Property(e => e.FuncionarioIdPrimeiroAtendimento)
            .HasColumnName("funcionario_primeiro_atendimento_id");

        builder.HasOne<Funcionario>()
            .WithMany()
            .HasForeignKey(e => e.FuncionarioIdPrimeiroAtendimento)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
        builder.Property(e => e.CreatedAt).HasColumnName("criado_em").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasIndex(e => e.WhatsAppPhoneNumberId)
            .IsUnique()
            .HasDatabaseName("ix_estabelecimentos_whatsapp_phone_number_id");

        builder.Ignore(e => e.DomainEvents);
    }
}
