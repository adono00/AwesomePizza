using AwesomePizza.Service.Dtos;

namespace AwesomePizza.Service.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request);
    Task<OrderResponse> GetByCodeAsync(string code);
    Task<List<OrderResponse>> GetQueueAsync();
    Task<OrderResponse> StartNextAsync();
    Task<OrderResponse> CompleteCurrentAsync();
    Task<OrderResponse> GetCurrentAsync();
}
