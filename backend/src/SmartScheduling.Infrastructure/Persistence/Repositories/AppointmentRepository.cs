using Microsoft.EntityFrameworkCore;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Infrastructure.Persistence.Repositories;

public class AgendamentoRepository(AppDbContext context) : Repository<Agendamento>(context), IAgendamentoRepository
{
    public async Task<IReadOnlyList<Agendamento>> ObterPorFuncionarioAsync(Guid funcionarioId, DateOnly data, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Servico)
            .Where(a => a.FuncionarioId == funcionarioId && a.Horario.Start.Date == data.ToDateTime(TimeOnly.MinValue).Date)
            .OrderBy(a => a.Horario.Start)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Agendamento>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Funcionario)
            .Include(a => a.Servico)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.Horario.Start)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Agendamento>> ObterPorEstabelecimentoAsync(Guid estabelecimentoId, DateOnly data, CancellationToken cancellationToken = default) =>
        await Context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Funcionario)
            .Include(a => a.Servico)
            .Where(a => a.EstabelecimentoId == estabelecimentoId && a.Horario.Start.Date == data.ToDateTime(TimeOnly.MinValue).Date)
            .OrderBy(a => a.Horario.Start)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Agendamento>> ObterProximosAsync(TimeSpan dentro, CancellationToken cancellationToken = default)
    {
        var ate = DateTime.UtcNow.Add(dentro);
        return await Context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Funcionario)
            .Include(a => a.Servico)
            .Where(a => a.Status == StatusAgendamento.Confirmado
                     && a.Horario.Start >= DateTime.UtcNow
                     && a.Horario.Start <= ate)
            .ToListAsync(cancellationToken);
    }
}
