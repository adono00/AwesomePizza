using AwesomePizza.Repository.Models;
using AwesomePizza.Service;
using AwesomePizza.Service.Dtos;
using AwesomePizza.Service.Exceptions;
using AwesomePizza.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AwesomePizza.Tests;

public class AuthServiceTests
{
    private readonly FakeUserRepository _users = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var hasher = new PasswordHasher<User>();
        var jwt = Options.Create(new JwtOptions
        {
            Key = new string('k', 40),
            Issuer = "test",
            Audience = "test"
        });

        _service = new AuthService(_users, hasher, jwt, NullLogger<AuthService>.Instance);

        var user = new User { Id = 1, UserName = "mario" };
        user.PasswordHash = hasher.HashPassword(user, "Segreta123!");
        _users.Users.Add(user);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsToken()
    {
        var response = await _service.LoginAsync(new LoginRequest { UserName = "Mario", Password = "Segreta123!" });

        Assert.False(string.IsNullOrEmpty(response.Token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ThrowsUnauthorized()
    {
        var request = new LoginRequest { UserName = "mario", Password = "sbagliata" };

        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(request));
    }

    [Fact]
    public async Task Login_UnknownUser_ThrowsUnauthorized()
    {
        var request = new LoginRequest { UserName = "nessuno", Password = "Segreta123!" };

        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(request));
    }
}