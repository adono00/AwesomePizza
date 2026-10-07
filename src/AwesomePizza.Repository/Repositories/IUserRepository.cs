using AwesomePizza.Repository.Models;

namespace AwesomePizza.Repository.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<bool> AnyAsync();
    Task AddAsync(User user);
}

