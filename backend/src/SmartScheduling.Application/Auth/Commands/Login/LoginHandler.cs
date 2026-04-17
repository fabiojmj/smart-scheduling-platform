using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Auth.Commands.Login;

public sealed class LoginHandler(IUsuarioRepository repo, ITokenService tokenService)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repo.ObterPorEmailAsync(request.Email.ToLowerInvariant(), cancellationToken)
            ?? throw new UnauthorizedException("Credenciais inválidas.");

        if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.PasswordHash))
            throw new UnauthorizedException("Credenciais inválidas.");

        var token = tokenService.GerarToken(usuario);
        return new LoginResponse(token, usuario.Nome, usuario.Email, usuario.Role.ToString());
    }
}
