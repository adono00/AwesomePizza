namespace AwesomePizza.Service.Dtos;

public record LoginResponse(string Token, DateTime ExpiresAt);