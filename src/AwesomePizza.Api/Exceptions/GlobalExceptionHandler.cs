using AwesomePizza.Service.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AwesomePizza.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Risorsa non trovata"),
            ConflictException => (StatusCodes.Status409Conflict, "Operazione non consentita"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Non autorizzato"),
            _ => (StatusCodes.Status500InternalServerError, "Errore interno")
        };

        // i dettagli degli errori imprevisti restano nei log, al client non li mostriamo
        string detail;
        if (status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Errore non gestito durante {Method} {Path}",
                context.Request.Method, context.Request.Path);
            detail = "Si è verificato un errore imprevisto.";
        }
        else
        {
            _logger.LogWarning("{Status} durante {Method} {Path}: {Message}",
                status, context.Request.Method, context.Request.Path, exception.Message);
            detail = exception.Message;
        }

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        }, ct);

        return true;
    }
}
