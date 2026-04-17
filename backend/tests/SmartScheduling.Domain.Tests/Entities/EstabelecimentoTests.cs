using FluentAssertions;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Tests.Entities;

public class EstabelecimentoTests
{
    [Fact]
    public void Criar_ComDadosValidos_CriaComSucesso()
    {
        var e = Estabelecimento.Criar("Salão Bela", "12345678901234", "proprietario-1");
        e.Id.Should().NotBeEmpty();
        e.Nome.Should().Be("Salão Bela");
        e.WhatsAppPhoneNumberId.Should().Be("12345678901234");
        e.ProprietarioId.Should().Be("proprietario-1");
        e.Ativo.Should().BeTrue();
        e.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazio_LancaDomainException(string nome)
    {
        var act = () => Estabelecimento.Criar(nome, "123", "prop");
        act.Should().Throw<DomainException>().WithMessage("*obrigatorio*");
    }

    [Fact]
    public void Criar_RemoveEspacosDoNome()
    {
        var e = Estabelecimento.Criar("  Salão  ", "123", "prop");
        e.Nome.Should().Be("Salão");
    }

    [Fact]
    public void AtualizarNome_ComNomeValido_AtualizaComSucesso()
    {
        var e = Estabelecimento.Criar("Salão Velho", "123", "prop");
        e.AtualizarNome("Salão Novo");
        e.Nome.Should().Be("Salão Novo");
        e.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AtualizarNome_ComNomeVazio_LancaDomainException(string nome)
    {
        var e = Estabelecimento.Criar("Salão", "123", "prop");
        var act = () => e.AtualizarNome(nome);
        act.Should().Throw<DomainException>().WithMessage("*obrigatorio*");
    }

    [Fact]
    public void Desativar_DefinirAtivoComoFalso()
    {
        var e = Estabelecimento.Criar("Salão", "123", "prop");
        e.Desativar();
        e.Ativo.Should().BeFalse();
        e.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AdicionarFuncionario_ComFuncionarioNovo_AdicionaComSucesso()
    {
        var e = Estabelecimento.Criar("Salão", "123", "prop");
        var f = Funcionario.Criar("Ana", "ana@test.com", e.Id);
        e.AdicionarFuncionario(f);
        e.Funcionarios.Should().Contain(f);
    }

    [Fact]
    public void AdicionarFuncionario_Duplicado_LancaDomainException()
    {
        var e = Estabelecimento.Criar("Salão", "123", "prop");
        var f = Funcionario.Criar("Ana", "ana@test.com", e.Id);
        e.AdicionarFuncionario(f);
        var act = () => e.AdicionarFuncionario(f);
        act.Should().Throw<DomainException>().WithMessage("*ja cadastrado*");
    }

    [Fact]
    public void AdicionarServico_ComServicoNovo_AdicionaComSucesso()
    {
        var e = Estabelecimento.Criar("Salão", "123", "prop");
        var s = Servico.Criar("Corte", 30, 50m, e.Id);
        e.AdicionarServico(s);
        e.Servicos.Should().Contain(s);
    }

    [Fact]
    public void AdicionarServico_ComNomeDuplicado_LancaDomainException()
    {
        var e = Estabelecimento.Criar("Salão", "123", "prop");
        var s1 = Servico.Criar("Corte", 30, 50m, e.Id);
        var s2 = Servico.Criar("Corte", 45, 60m, e.Id);
        e.AdicionarServico(s1);
        var act = () => e.AdicionarServico(s2);
        act.Should().Throw<DomainException>().WithMessage("*ja existe*");
    }

    [Fact]
    public void DomainEvents_InicialmenteVazios()
    {
        var e = Estabelecimento.Criar("Salão", "123", "prop");
        e.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_RemoveTodosOsEventos()
    {
        var e = Estabelecimento.Criar("Salão", "123", "prop");
        e.ClearDomainEvents();
        e.DomainEvents.Should().BeEmpty();
    }
}
