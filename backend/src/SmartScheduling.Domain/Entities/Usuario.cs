using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.Domain.Entities;

public enum Role { Proprietario, Admin }

public class Usuario : Entity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Role Role { get; private set; }

    private Usuario() { Nome = default!; Email = default!; PasswordHash = default!; }

    public static Usuario Criar(string nome, string email, string passwordHash, Role role = Role.Proprietario)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DomainException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email é obrigatório.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("Senha é obrigatória.");
        return new Usuario { Nome = nome.Trim(), Email = email.Trim().ToLowerInvariant(), PasswordHash = passwordHash, Role = role };
    }

    public void AtualizarSenha(string passwordHash) { PasswordHash = passwordHash; MarkAsUpdated(); }
}
