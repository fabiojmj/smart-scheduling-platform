using FluentAssertions;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Domain.Tests.Entities;

public class AgendamentoTests
{
    private static DateTime NextWeekday(DayOfWeek day, int hour = 10)
    {
        var now = DateTime.UtcNow;
        int daysUntil = ((int)day - (int)now.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7;
        return now.Date.AddDays(daysUntil).AddHours(hour);
    }

    private static (Cliente, Funcionario, Servico, TimeSlot) CriarSetupValido()
    {
        var estId = Guid.NewGuid();
        var cliente = Cliente.Criar("João", "11987654321", estId);
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", estId);
        var servico = Servico.Criar("Corte", 30, 50m, estId);

        funcionario.AtribuirServico(servico);
        funcionario.AdicionarHorario(WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));

        var slot = TimeSlot.Create(NextWeekday(DayOfWeek.Monday), 30);
        return (cliente, funcionario, servico, slot);
    }

    [Fact]
    public void Agendar_ComDadosValidos_CriaComSucesso()
    {
        var (cliente, funcionario, servico, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(cliente, funcionario, servico, slot, "Obs");

        a.ClienteId.Should().Be(cliente.Id);
        a.FuncionarioId.Should().Be(funcionario.Id);
        a.ServicoId.Should().Be(servico.Id);
        a.EstabelecimentoId.Should().Be(servico.EstabelecimentoId);
        a.Status.Should().Be(StatusAgendamento.Pendente);
        a.Observacoes.Should().Be("Obs");
        a.MotivoCancelamento.Should().BeNull();
        a.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Agendar_ComFuncionarioSemHorario_LancaSchedulingConflictException()
    {
        var estId = Guid.NewGuid();
        var cliente = Cliente.Criar("João", "11987654321", estId);
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", estId);
        var servico = Servico.Criar("Corte", 30, 50m, estId);
        funcionario.AtribuirServico(servico);
        // NO working hours added

        var slot = TimeSlot.Create(NextWeekday(DayOfWeek.Monday), 30);
        var act = () => Agendamento.Agendar(cliente, funcionario, servico, slot);
        act.Should().Throw<SchedulingConflictException>();
    }

    [Fact]
    public void Agendar_ComFuncionarioSemServico_LancaDomainException()
    {
        var estId = Guid.NewGuid();
        var cliente = Cliente.Criar("João", "11987654321", estId);
        var funcionario = Funcionario.Criar("Ana", "ana@test.com", estId);
        var servico = Servico.Criar("Corte", 30, 50m, estId);
        funcionario.AdicionarHorario(WorkingHours.Create(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(18, 0)));
        // Service NOT assigned

        var slot = TimeSlot.Create(NextWeekday(DayOfWeek.Monday), 30);
        var act = () => Agendamento.Agendar(cliente, funcionario, servico, slot);
        act.Should().Throw<DomainException>().WithMessage("*nao executa*");
    }

    [Fact]
    public void Confirmar_QuandoPendente_ConfirmaComSucesso()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Confirmar();
        a.Status.Should().Be(StatusAgendamento.Confirmado);
        a.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Confirmar_QuandoNaoPendente_LancaDomainException()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Confirmar();
        var act = () => a.Confirmar();
        act.Should().Throw<DomainException>().WithMessage("*pendentes*");
    }

    [Fact]
    public void Concluir_QuandoConfirmado_ConcluidoComSucesso()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Confirmar();
        a.Concluir();
        a.Status.Should().Be(StatusAgendamento.Concluido);
        a.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Concluir_QuandoNaoConfirmado_LancaDomainException()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        var act = () => a.Concluir();
        act.Should().Throw<DomainException>().WithMessage("*confirmados*");
    }

    [Fact]
    public void Cancelar_QuandoPendente_CancelaComSucesso()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Cancelar("cliente desistiu");
        a.Status.Should().Be(StatusAgendamento.Cancelado);
        a.MotivoCancelamento.Should().Be("cliente desistiu");
        a.UpdatedAt.Should().NotBeNull();
        a.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void Cancelar_QuandoConfirmado_CancelaComSucesso()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Confirmar();
        a.Cancelar("cliente desistiu");
        a.Status.Should().Be(StatusAgendamento.Cancelado);
    }

    [Fact]
    public void Cancelar_QuandoConcluido_LancaDomainException()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Confirmar();
        a.Concluir();
        var act = () => a.Cancelar("motivo");
        act.Should().Throw<DomainException>().WithMessage("*finalizado*");
    }

    [Fact]
    public void Cancelar_QuandoJaCancelado_LancaDomainException()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Cancelar("motivo 1");
        var act = () => a.Cancelar("motivo 2");
        act.Should().Throw<DomainException>().WithMessage("*finalizado*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Cancelar_ComMotivoVazio_LancaDomainException(string motivo)
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        var act = () => a.Cancelar(motivo);
        act.Should().Throw<DomainException>().WithMessage("*obrigatorio*");
    }

    [Fact]
    public void MarcarNaoCompareceu_QuandoConfirmado_MarcaComSucesso()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Confirmar();
        a.MarcarNaoCompareceu();
        a.Status.Should().Be(StatusAgendamento.NaoCompareceu);
        a.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarcarNaoCompareceu_QuandoNaoConfirmado_LancaDomainException()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        var act = () => a.MarcarNaoCompareceu();
        act.Should().Throw<DomainException>().WithMessage("*confirmados*");
    }

    [Theory]
    [InlineData(StatusAgendamento.Pendente, true)]
    [InlineData(StatusAgendamento.Confirmado, true)]
    public void EstaAtivo_QuandoAtivo_RetornaVerdadeiro(StatusAgendamento status, bool esperado)
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        if (status == StatusAgendamento.Confirmado) a.Confirmar();
        a.EstaAtivo().Should().Be(esperado);
    }

    [Theory]
    [InlineData(false)]
    public void EstaAtivo_QuandoCancelado_RetornaFalso(bool esperado)
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Cancelar("teste");
        a.EstaAtivo().Should().Be(esperado);
    }

    [Fact]
    public void EstaAtivo_QuandoConcluido_RetornaFalso()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Confirmar();
        a.Concluir();
        a.EstaAtivo().Should().BeFalse();
    }

    [Fact]
    public void EstaAtivo_QuandoNaoCompareceu_RetornaFalso()
    {
        var (c, f, s, slot) = CriarSetupValido();
        var a = Agendamento.Agendar(c, f, s, slot);
        a.Confirmar();
        a.MarcarNaoCompareceu();
        a.EstaAtivo().Should().BeFalse();
    }
}
