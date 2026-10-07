using AwesomePizza.Repository.Models;

namespace AwesomePizza.Repository.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task<Order?> GetByCodeAsync(string code);
    Task<Order?> GetInProgressAsync();
    Task<Order?> GetNextInQueueAsync();
    Task<List<Order>> GetQueueAsync();
}
