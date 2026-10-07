using AwesomePizza.Service.Dtos;

namespace AwesomePizza.Service.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}