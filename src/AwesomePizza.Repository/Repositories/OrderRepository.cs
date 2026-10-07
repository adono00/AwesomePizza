using AwesomePizza.Repository.Data;
using AwesomePizza.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace AwesomePizza.Repository.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly PizzaDbContext _db;

    public OrderRepository(PizzaDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
    }

    public Task<Order?> GetByCodeAsync(string code)
    {
        return _db.Orders.FirstOrDefaultAsync(o => o.Code == code);
    }

    public Task<Order?> GetInProgressAsync()
    {
        return _db.Orders.FirstOrDefaultAsync(o => o.Status == OrderStatus.InProgress);
    }

    // l'ordine più vecchio tra quelli ancora da lavorare
    public Task<Order?> GetNextInQueueAsync()
    {
        return _db.Orders
            .Where(o => o.Status == OrderStatus.Received)
            .OrderBy(o => o.Id)
            .FirstOrDefaultAsync();
    }

    public Task<List<Order>> GetQueueAsync()
    {
        return _db.Orders
            .Where(o => o.Status == OrderStatus.Received)
            .OrderBy(o => o.Id)
            .ToListAsync();
    }
}
