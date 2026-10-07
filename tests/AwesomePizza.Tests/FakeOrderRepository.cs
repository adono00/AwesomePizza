using AwesomePizza.Repository.Models;
using AwesomePizza.Repository.Repositories;

namespace AwesomePizza.Tests;

// repository finto che tiene tutto in memoria, così i test non toccano il database
public class FakeOrderRepository : IOrderRepository
{
    public List<Order> Orders { get; } = new();

    public Task AddAsync(Order order)
    {
        order.Id = Orders.Count + 1;
        Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order) => Task.CompletedTask;

    public Task<Order?> GetByCodeAsync(string code) =>
        Task.FromResult(Orders.FirstOrDefault(o => o.Code == code));

    public Task<Order?> GetInProgressAsync() =>
        Task.FromResult(Orders.FirstOrDefault(o => o.Status == OrderStatus.InProgress));

    public Task<Order?> GetNextInQueueAsync() =>
        Task.FromResult(Orders.Where(o => o.Status == OrderStatus.Received).OrderBy(o => o.Id).FirstOrDefault());

    public Task<List<Order>> GetQueueAsync() =>
        Task.FromResult(Orders.Where(o => o.Status == OrderStatus.Received).OrderBy(o => o.Id).ToList());
}
