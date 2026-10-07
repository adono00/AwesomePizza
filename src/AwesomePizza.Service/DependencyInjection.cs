using AwesomePizza.Repository.Models;
using AwesomePizza.Repository.Repositories;
using AwesomePizza.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AwesomePizza.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        return services;
    }

    // se non esiste nessun utente crea il pizzaiolo di default (con la password in hash)
    public static async Task SeedUsersAsync(this IServiceProvider provider, string userName, string password)
    {
        using var scope = provider.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        if (await users.AnyAsync())
            return;

        var user = new User { UserName = userName.Trim().ToLowerInvariant() };
        user.PasswordHash = hasher.HashPassword(user, password);
        await users.AddAsync(user);
    }
}