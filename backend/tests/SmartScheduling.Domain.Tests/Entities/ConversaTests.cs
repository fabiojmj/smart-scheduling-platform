using FluentAssertions;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Enums;

namespace SmartScheduling.Domain.Tests.Entities;

public class ConversaTests
{
    [Fact]
    public void Iniciar_ComDadosValidos_CriaComSucesso()
    {
        var estId = Guid.NewGuid();
        var c = Conversa.Iniciar("11987654321", estId);
        c.TelefoneCliente.Value.Should().Be("11987654321");
        c.EstabelecimentoId.Should().Be(estId);
        c.Status.Should().Be(StatusConversa.Ativa);
        c.AgendamentoPendenteId.Should().BeNull();
        c.Mensagens.Should().BeEmpty();
    }

    [Fact]
    public void AdicionarMensagem_CriaEAdicionaMensagem()
    {
        var c = Conversa.Iniciar("11987654321", Guid.NewGuid());
        c.AdicionarMensagem("Olá", TipoMensagem.Texto, true);
        c.Mensagens.Should().HaveCount(1);
        c.Mensagens.First().Conteudo.Should().Be("Olá");
        c.Mensagens.First().VeioDoCliente.Should().BeTrue();
    }

    [Fact]
    public void AguardarConfirmacao_AtualizaStatusEId()
    {
        var c = Conversa.Iniciar("11987654321", Guid.NewGuid());
        var agId = Guid.NewGuid();
        c.AguardarConfirmacao(agId);
        c.Status.Should().Be(StatusConversa.AguardandoConfirmacao);
        c.AgendamentoPendenteId.Should().Be(agId);
        c.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Concluir_AtualizaStatusParaConcluida()
    {
        var c = Conversa.Iniciar("11987654321", Guid.NewGuid());
        c.Concluir();
        c.Status.Should().Be(StatusConversa.Concluida);
        c.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Abandonar_AtualizaStatusParaAbandonada()
    {
        var c = Conversa.Iniciar("11987654321", Guid.NewGuid());
        c.Abandonar();
        c.Status.Should().Be(StatusConversa.Abandonada);
        c.UpdatedAt.Should().NotBeNull();
    }
}
