using FluentAssertions;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Tests.Entities;

public class ServicoTests
{
    [Fact]
    public void Criar_ComDadosValidos_CriaComSucesso()
    {
        var id = Guid.NewGuid();
        var s = Servico.Criar("Corte", 30, 50m, id, "Corte masculino");
        s.Nome.Should().Be("Corte");
        s.DuracaoMinutos.Should().Be(30);
        s.Preco.Should().Be(50m);
        s.EstabelecimentoId.Should().Be(id);
        s.Descricao.Should().Be("Corte masculino");
        s.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Criar_SemDescricao_DescricaoNula()
    {
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        s.Descricao.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazio_LancaDomainException(string nome)
    {
        var act = () => Servico.Criar(nome, 30, 50m, Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*obrigatorio*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Criar_ComDuracaoInvalida_LancaDomainException(int duracao)
    {
        var act = () => Servico.Criar("Corte", duracao, 50m, Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*maior que zero*");
    }

    [Fact]
    public void Criar_ComPrecoNegativo_LancaDomainException()
    {
        var act = () => Servico.Criar("Corte", 30, -1m, Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*negativo*");
    }

    [Fact]
    public void Criar_ComPrecoZero_CriaComSucesso()
    {
        var s = Servico.Criar("Corte Gratis", 30, 0m, Guid.NewGuid());
        s.Preco.Should().Be(0m);
    }

    [Fact]
    public void AtualizarPreco_ComValorValido_AtualizaComSucesso()
    {
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        s.AtualizarPreco(80m);
        s.Preco.Should().Be(80m);
        s.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarPreco_ComValorNegativo_LancaDomainException()
    {
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        var act = () => s.AtualizarPreco(-1m);
        act.Should().Throw<DomainException>().WithMessage("*invalido*");
    }

    [Fact]
    public void AtualizarDuracao_ComValorValido_AtualizaComSucesso()
    {
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        s.AtualizarDuracao(60);
        s.DuracaoMinutos.Should().Be(60);
        s.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AtualizarDuracao_ComValorInvalido_LancaDomainException(int minutos)
    {
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        var act = () => s.AtualizarDuracao(minutos);
        act.Should().Throw<DomainException>().WithMessage("*invalida*");
    }

    [Fact]
    public void Desativar_DefinirAtivoComoFalso()
    {
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        s.Desativar();
        s.Ativo.Should().BeFalse();
        s.UpdatedAt.Should().NotBeNull();
    }
}
