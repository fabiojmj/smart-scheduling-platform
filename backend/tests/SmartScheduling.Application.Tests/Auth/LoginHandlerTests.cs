using FluentAssertions;
using Moq;
using SmartScheduling.Application.Auth.Commands.Login;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Auth;

public class LoginHandlerTests
{
    private readonly Mock<IUsuarioRepository> _repoMock = new();
    private readonly Mock<ITokenService> _tokenMock = new();
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _handler = new LoginHandler(_repoMock.Object, _tokenMock.Object);
    }

    [Fact]
    public async Task Handle_CredenciaisValidas_RetornaLoginResponse()
    {
        var senha = "senha123";
        var hash = BCrypt.Net.BCrypt.HashPassword(senha);
        var usuario = Usuario.Criar("João", "joao@test.com", hash);

        _repoMock.Setup(r => r.ObterPorEmailAsync("joao@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _tokenMock.Setup(t => t.GerarToken(usuario)).Returns("token-jwt");

        var result = await _handler.Handle(new LoginCommand("joao@test.com", senha), CancellationToken.None);

        result.Token.Should().Be("token-jwt");
        result.Nome.Should().Be("João");
        result.Email.Should().Be("joao@test.com");
        result.Role.Should().Be("Proprietario");
    }

    [Fact]
    public async Task Handle_EmailConverteParaMinusculas_AntesDeConsultar()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("senha123");
        var usuario = Usuario.Criar("João", "joao@test.com", hash);

        _repoMock.Setup(r => r.ObterPorEmailAsync("joao@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _tokenMock.Setup(t => t.GerarToken(It.IsAny<Usuario>())).Returns("token");

        await _handler.Handle(new LoginCommand("JOAO@TEST.COM", "senha123"), CancellationToken.None);

        _repoMock.Verify(r => r.ObterPorEmailAsync("joao@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UsuarioNaoEncontrado_LancaUnauthorizedException()
    {
        _repoMock.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var act = () => _handler.Handle(new LoginCommand("x@x.com", "senha"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Credenciais inválidas.");
    }

    [Fact]
    public async Task Handle_SenhaErrada_LancaUnauthorizedException()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("correta");
        var usuario = Usuario.Criar("João", "joao@test.com", hash);

        _repoMock.Setup(r => r.ObterPorEmailAsync("joao@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var act = () => _handler.Handle(new LoginCommand("joao@test.com", "errada"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Credenciais inválidas.");
    }
}
