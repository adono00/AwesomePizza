using AwesomePizza.Repository.Data;
using AwesomePizza.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace AwesomePizza.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PizzaDbContext _db;

    public UserRepository(PizzaDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByUserNameAsync(string userName)
    {
        return _db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public Task<bool> AnyAsync()
    {
        return _db.Users.AnyAsync();
    }

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}