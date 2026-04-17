using MediatR;

namespace SmartScheduling.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Senha) : IRequest<LoginResponse>;

public record LoginResponse(string Token, string Nome, string Email, string Role);
