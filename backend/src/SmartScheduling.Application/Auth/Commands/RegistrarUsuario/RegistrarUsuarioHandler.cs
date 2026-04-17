using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Auth.Commands.RegistrarUsuario;

public sealed class RegistrarUsuarioHandler(IUsuarioRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<RegistrarUsuarioCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (await repo.EmailExisteAsync(request.Email, cancellationToken))
            throw new DomainException("Email já cadastrado.");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
        var usuario = Usuario.Criar(request.Nome, request.Email, hash);

        await repo.AddAsync(usuario, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return usuario.Id;
    }
}
