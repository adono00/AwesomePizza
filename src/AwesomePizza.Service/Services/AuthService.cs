using System.Security.Claims;
using System.Text;
using AwesomePizza.Repository.Models;
using AwesomePizza.Repository.Repositories;
using AwesomePizza.Service.Dtos;
using AwesomePizza.Service.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AwesomePizza.Service.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtOptions _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher,
        IOptions<JwtOptions> jwt, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwt = jwt.Value;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var userName = request.UserName.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByUserNameAsync(userName);

        // stesso errore per utente inesistente e password sbagliata: non diciamo quale dei due è errato
        if (user is null || !PasswordIsValid(user, request.Password))
        {
            _logger.LogWarning("Login fallito per l'utente {UserName}", userName);
            throw new UnauthorizedException("Credenziali non valide.");
        }

        _logger.LogInformation("Login riuscito per l'utente {UserName}", user.UserName);
        return BuildToken(user);
    }

    private bool PasswordIsValid(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result != PasswordVerificationResult.Failed;
    }

    private LoginResponse BuildToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiresMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, Roles.Pizzaiolo)
            })
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return new LoginResponse(token, expiresAt);
    }
}