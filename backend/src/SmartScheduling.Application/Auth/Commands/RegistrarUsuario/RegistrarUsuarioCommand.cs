using MediatR;

namespace SmartScheduling.Application.Auth.Commands.RegistrarUsuario;

public record RegistrarUsuarioCommand(string Nome, string Email, string Senha) : IRequest<Guid>;
