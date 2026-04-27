using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Infrastructure.Persistence;
using SmartScheduling.Infrastructure.Persistence.Repositories;
using SmartScheduling.Infrastructure.Services;

namespace SmartScheduling.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
        services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<IConversaRepository, ConversaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IEstablishmentWorkingHoursRepository, EstablishmentWorkingHoursRepository>();
        services.AddScoped<IEstablishmentBlockRepository, EstablishmentBlockRepository>();
        services.AddScoped<IRecurringScheduleRepository, RecurringScheduleRepository>();
        services.AddScoped<IEstablishmentAvailabilityService, EstablishmentAvailabilityService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
