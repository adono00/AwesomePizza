using AwesomePizza.Repository.Data;
using AwesomePizza.Repository.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AwesomePizza.Repository.Logging;

public class DatabaseLoggerProvider : ILoggerProvider
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseLoggerProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public ILogger CreateLogger(string categoryName) => new DatabaseLogger(categoryName, _scopeFactory);

    public void Dispose() { }
}

public class DatabaseLogger : ILogger
{
    private readonly string _category;
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseLogger(string category, IServiceScopeFactory scopeFactory)
    {
        _category = category;
        _scopeFactory = scopeFactory;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    
    public bool IsEnabled(LogLevel logLevel) =>
        _category.StartsWith("AwesomePizza") && logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PizzaDbContext>();

            db.Logs.Add(new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = logLevel.ToString(),
                Category = _category,
                Message = formatter(state, exception),
                Exception = exception?.ToString()
            });
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Impossibile salvare il log su database: {ex.Message}");
        }
    }
}