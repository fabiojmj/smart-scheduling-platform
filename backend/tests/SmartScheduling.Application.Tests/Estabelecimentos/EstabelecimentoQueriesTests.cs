using FluentAssertions;
using Moq;
using SmartScheduling.Application.Estabelecimentos.Queries.ObterEstabelecimento;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Tests.Estabelecimentos;

public class ObterEstabelecimentoHandlerTests
{
    private readonly Mock<IEstabelecimentoRepository> _repoMock = new();
    private readonly ObterEstabelecimentoHandler _handler;

    public ObterEstabelecimentoHandlerTests()
    {
        _handler = new ObterEstabelecimentoHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_EstabelecimentoExistente_RetornaDto()
    {
        var est = Estabelecimento.Criar("Barbearia Top", "wa-id-123", "prop-abc");
        _repoMock.Setup(r => r.GetByIdAsync(est.Id, It.IsAny<CancellationToken>())).ReturnsAsync(est);

        var result = await _handler.Handle(new ObterEstabelecimentoQuery(est.Id), CancellationToken.None);

        result.Id.Should().Be(est.Id);
        result.Nome.Should().Be("Barbearia Top");
        result.WhatsAppPhoneNumberId.Should().Be("wa-id-123");
        result.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EstabelecimentoNaoEncontrado_LancaDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Estabelecimento?)null);

        var act = () => _handler.Handle(new ObterEstabelecimentoQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Estabelecimento*");
    }
}
