using AwesomePizza.Repository.Data;
using AwesomePizza.Repository.Logging;
using AwesomePizza.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AwesomePizza.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PizzaDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }

    public static ILoggingBuilder AddDatabaseLogging(this ILoggingBuilder logging)
    {
        logging.Services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();
        return logging;
    }

    // all'avvio crea il database (se manca) e applica le migration
    public static void MigrateDatabase(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<PizzaDbContext>().Database.Migrate();
    }
}
