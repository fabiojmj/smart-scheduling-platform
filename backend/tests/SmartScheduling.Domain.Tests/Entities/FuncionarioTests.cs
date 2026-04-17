using System.Reflection;
using FluentAssertions;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Tests.Entities;

public class FuncionarioTests
{
    private static DateTime NextWeekday(DayOfWeek day, int hour = 10)
    {
        var now = DateTime.UtcNow;
        int daysUntil = ((int)day - (int)now.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7;
        return now.Date.AddDays(daysUntil).AddHours(hour);
    }

    [Fact]
    public void Criar_ComDadosValidos_CriaComSucesso()
    {
        var id = Guid.NewGuid();
        var f = Funcionario.Criar("Ana Costa", "ana@test.com", id);
        f.Nome.Should().Be("Ana Costa");
        f.Email.Should().Be("ana@test.com");
        f.EstabelecimentoId.Should().Be(id);
        f.Ativo.Should().BeTrue();
        f.Horarios.Should().BeEmpty();
        f.Servicos.Should().BeEmpty();
        f.Agendamentos.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazio_LancaDomainException(string nome)
    {
        var act = () => Funcionario.Criar(nome, "email@test.com", Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*obrigatorio*");
    }

    [Fact]
    public void Criar_EmailConvertidoParaMinusculas()
    {
        var f = Funcionario.Criar("Ana", "ANA@TEST.COM", Guid.NewGuid());
        f.Email.Should().Be("ana@test.com");
    }

    [Fact]
    public void AdicionarHorario_ComHorarioNovo_AdicionaComSucesso()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        var wh = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0));
        f.AdicionarHorario(wh);
        f.Horarios.Should().HaveCount(1).And.Contain(wh);
    }

    [Fact]
    public void AdicionarHorario_ComDiaDuplicado_LancaDomainException()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        var wh1 = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0));
        var wh2 = WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(13, 0), new TimeOnly(18, 0));
        f.AdicionarHorario(wh1);
        var act = () => f.AdicionarHorario(wh2);
        act.Should().Throw<DomainException>().WithMessage("*ja configurado*");
    }

    [Fact]
    public void AtribuirServico_ComServicoNovo_AtribuiComSucesso()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        f.AtribuirServico(s);
        f.Servicos.Should().Contain(s);
    }

    [Fact]
    public void AtribuirServico_Duplicado_LancaDomainException()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        f.AtribuirServico(s);
        var act = () => f.AtribuirServico(s);
        act.Should().Throw<DomainException>().WithMessage("*ja atribuido*");
    }

    [Fact]
    public void PodeExecutar_QuandoServicoAtribuido_RetornaVerdadeiro()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        f.AtribuirServico(s);
        f.PodeExecutar(s).Should().BeTrue();
    }

    [Fact]
    public void PodeExecutar_QuandoServicoNaoAtribuido_RetornaFalso()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        var s = Servico.Criar("Corte", 30, 50m, Guid.NewGuid());
        f.PodeExecutar(s).Should().BeFalse();
    }

    [Fact]
    public void EstaDisponivel_QuandoSemHorario_RetornaFalso()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        var slot = TimeSlot.Create(NextWeekday(DayOfWeek.Monday), 30);
        f.EstaDisponivel(slot).Should().BeFalse();
    }

    [Fact]
    public void EstaDisponivel_QuandoTemHorarioESemConflito_RetornaVerdadeiro()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        f.AdicionarHorario(WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));
        var slot = TimeSlot.Create(NextWeekday(DayOfWeek.Monday), 30);
        f.EstaDisponivel(slot).Should().BeTrue();
    }

    [Fact]
    public void EstaDisponivel_QuandoTemAgendamentoConflitante_RetornaFalso()
    {
        var estId = Guid.NewGuid();
        var f = Funcionario.Criar("Ana", "ana@test.com", estId);
        f.AdicionarHorario(WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));
        var s = Servico.Criar("Corte", 60, 50m, estId);
        f.AtribuirServico(s);

        var slotStart = NextWeekday(DayOfWeek.Monday);
        var slot1 = TimeSlot.Create(slotStart, 60);
        var slot2 = TimeSlot.Create(slotStart.AddMinutes(30), 60);

        var cliente = Cliente.Criar("Bob", "11987654321", estId);
        var agendamento = Agendamento.Agendar(cliente, f, s, slot1);

        // Populate private _agendamentos via reflection to simulate EF loading
        var field = typeof(Funcionario).GetField("_agendamentos", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var list = (List<Agendamento>)field.GetValue(f)!;
        list.Add(agendamento);

        f.EstaDisponivel(slot2).Should().BeFalse();
    }

    [Fact]
    public void Desativar_DefinirAtivoComoFalso()
    {
        var f = Funcionario.Criar("Ana", "ana@test.com", Guid.NewGuid());
        f.Desativar();
        f.Ativo.Should().BeFalse();
        f.UpdatedAt.Should().NotBeNull();
    }
}
