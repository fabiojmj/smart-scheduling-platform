using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using SmartScheduling.Application.Common.Behaviors;

namespace SmartScheduling.Application.Tests.Common;

public record TestValidationRequest(string Value) : IRequest<string>;

public class TestValidationRequestValidator : AbstractValidator<TestValidationRequest>
{
    public TestValidationRequestValidator()
    {
        RuleFor(x => x.Value).NotEmpty().WithMessage("Value é obrigatório.");
    }
}

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_SemValidadores_PassaParaProximoHandler()
    {
        var behavior = new ValidationBehavior<TestValidationRequest, string>(
            Enumerable.Empty<IValidator<TestValidationRequest>>());
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n()).ReturnsAsync("resultado");

        var result = await behavior.Handle(new TestValidationRequest("ok"), nextMock.Object, CancellationToken.None);

        result.Should().Be("resultado");
        nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidacaoPassando_PassaParaProximoHandler()
    {
        var validator = new TestValidationRequestValidator();
        var behavior = new ValidationBehavior<TestValidationRequest, string>(new[] { validator });
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n()).ReturnsAsync("resultado");

        var result = await behavior.Handle(new TestValidationRequest("valor-ok"), nextMock.Object, CancellationToken.None);

        result.Should().Be("resultado");
        nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidacaoFalhando_LancaValidationException()
    {
        var validator = new TestValidationRequestValidator();
        var behavior = new ValidationBehavior<TestValidationRequest, string>(new[] { validator });
        var nextMock = new Mock<RequestHandlerDelegate<string>>();

        var act = () => behavior.Handle(new TestValidationRequest(""), nextMock.Object, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        nextMock.Verify(n => n(), Times.Never);
    }
}
