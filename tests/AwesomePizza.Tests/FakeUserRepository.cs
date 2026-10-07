using AwesomePizza.Repository.Models;
using AwesomePizza.Repository.Repositories;

namespace AwesomePizza.Tests;

public class FakeUserRepository : IUserRepository
{
    public List<User> Users { get; } = new();

    public Task<User?> GetByUserNameAsync(string userName) =>
        Task.FromResult(Users.FirstOrDefault(u => u.UserName == userName));

    public Task<bool> AnyAsync() => Task.FromResult(Users.Count > 0);

    public Task AddAsync(User user)
    {
        Users.Add(user);
        return Task.CompletedTask;
    }
}