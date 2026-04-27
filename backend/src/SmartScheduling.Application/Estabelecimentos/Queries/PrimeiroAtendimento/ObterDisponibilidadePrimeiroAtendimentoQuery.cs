using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Estabelecimentos.Queries.PrimeiroAtendimento;

public record SlotDisponivelDto(DateTime Inicio, DateTime Fim);

public record ObterDisponibilidadePrimeiroAtendimentoResult(
    bool Configurado,
    Guid? FuncionarioId,
    string? FuncionarioNome,
    IReadOnlyList<SlotDisponivelDto> SlotsDisponiveis
);

public record ObterDisponibilidadePrimeiroAtendimentoQuery(
    Guid EstabelecimentoId,
    Guid ServicoId,
    DateOnly Data
) : IRequest<ObterDisponibilidadePrimeiroAtendimentoResult>;

public sealed class ObterDisponibilidadePrimeiroAtendimentoHandler(
    IEstabelecimentoRepository estabelecimentoRepo,
    IFuncionarioRepository funcionarioRepo,
    IServicoRepository servicoRepo,
    IAgendamentoRepository agendamentoRepo)
    : IRequestHandler<ObterDisponibilidadePrimeiroAtendimentoQuery, ObterDisponibilidadePrimeiroAtendimentoResult>
{
    public async Task<ObterDisponibilidadePrimeiroAtendimentoResult> Handle(
        ObterDisponibilidadePrimeiroAtendimentoQuery request,
        CancellationToken cancellationToken)
    {
        var estabelecimento = await estabelecimentoRepo.GetByIdAsync(request.EstabelecimentoId, cancellationToken)
            ?? throw new DomainException("Estabelecimento nao encontrado.");

        if (estabelecimento.FuncionarioIdPrimeiroAtendimento is null)
            return new ObterDisponibilidadePrimeiroAtendimentoResult(false, null, null, []);

        var servico = await servicoRepo.GetByIdAsync(request.ServicoId, cancellationToken)
            ?? throw new DomainException("Servico nao encontrado.");

        var funcionario = await funcionarioRepo.GetByIdAsync(estabelecimento.FuncionarioIdPrimeiroAtendimento.Value, cancellationToken)
            ?? throw new DomainException("Funcionario do primeiro atendimento nao encontrado.");

        if (!funcionario.Ativo)
            return new ObterDisponibilidadePrimeiroAtendimentoResult(true, funcionario.Id, funcionario.Nome, []);

        var horarioDia = funcionario.Horarios.FirstOrDefault(h => h.DayOfWeek == request.Data.DayOfWeek);
        if (horarioDia is null)
            return new ObterDisponibilidadePrimeiroAtendimentoResult(true, funcionario.Id, funcionario.Nome, []);

        var agendamentos = await agendamentoRepo.ObterPorFuncionarioAsync(
            funcionario.Id, request.Data, cancellationToken);

        var slots = new List<SlotDisponivelDto>();
        var inicio = request.Data.ToDateTime(horarioDia.Start);
        var fim = request.Data.ToDateTime(horarioDia.End);
        var duracao = TimeSpan.FromMinutes(servico.DuracaoMinutos);

        for (var candidato = inicio; candidato + duracao <= fim; candidato += duracao)
        {
            var slot = TimeSlot.Create(candidato, servico.DuracaoMinutos);
            bool ocupado = agendamentos.Any(a => a.EstaAtivo() && a.Horario.OverlapsWith(slot));
            if (!ocupado)
                slots.Add(new SlotDisponivelDto(slot.Start, slot.End));
        }

        return new ObterDisponibilidadePrimeiroAtendimentoResult(true, funcionario.Id, funcionario.Nome, slots);
    }
}
