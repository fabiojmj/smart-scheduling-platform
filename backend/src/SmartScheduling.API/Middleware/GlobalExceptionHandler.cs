using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartScheduling.Domain.Exceptions;

namespace SmartScheduling.API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Excecao: {Message}", exception.Message);
        var pd = exception switch
        {
            DomainException        => new ProblemDetails { Status = 422, Title = "Erro de dominio",    Detail = exception.Message },
            ValidationException ve => new ProblemDetails { Status = 400, Title = "Erro de validacao",  Detail = string.Join("; ", ve.Errors.Select(e => e.ErrorMessage)) },
            _                      => new ProblemDetails { Status = 500, Title = "Erro interno",       Detail = "Ocorreu um erro inesperado." }
        };
        context.Response.StatusCode = pd.Status!.Value;
        await context.Response.WriteAsJsonAsync(pd, cancellationToken);
        return true;
    }
}
