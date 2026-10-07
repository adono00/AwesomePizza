using AwesomePizza.Repository.Models;
using AwesomePizza.Repository.Repositories;
using AwesomePizza.Service.Dtos;
using AwesomePizza.Service.Exceptions;
using Microsoft.Extensions.Logging;

namespace AwesomePizza.Service.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository repository, ILogger<OrderService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            Code = GenerateCode(),
            CustomerName = request.CustomerName.Trim(),
            Items = request.Items
                .Select(i => new OrderItem { PizzaName = i.PizzaName.Trim(), Quantity = i.Quantity, Notes = i.Notes.Trim() })
                .ToList()
        };

        await _repository.AddAsync(order);

        var notes = order.Items
            .Where(i => !string.IsNullOrWhiteSpace(i.Notes))
            .Select(i => $"{i.PizzaName}: {i.Notes}")
            .ToList();
        var noteText = notes.Count > 0 ? $" | note: {string.Join(", ", notes)}" : "";
        _logger.LogInformation("Ordine {Code} creato per {Customer} con {Pizze} pizze{Note}",
            order.Code, order.CustomerName, order.Items.Sum(i => i.Quantity), noteText);

        return ToResponse(order);
    }

    public async Task<OrderResponse> GetByCodeAsync(string code)
    {
        var order = await _repository.GetByCodeAsync(code.Trim().ToUpper())
            ?? throw new NotFoundException($"Nessun ordine con codice {code}.");

        return ToResponse(order);
    }

    public async Task<List<OrderResponse>> GetQueueAsync()
    {
        var queue = await _repository.GetQueueAsync();
        _logger.LogInformation("Coda consultata: {Count} ordini in attesa", queue.Count);

        return queue.Select(ToResponse).ToList();
    }

    public async Task<OrderResponse> GetCurrentAsync()
    {
        var order = await _repository.GetInProgressAsync();
        if(order is null)
            throw new NotFoundException("Non ci sono ordini in preparazione.");
        _logger.LogInformation("L'ordine {Code} è quello attualmente in preparazione", order.Code);

        return ToResponse(order);
    }

    public async Task<OrderResponse> StartNextAsync()
    {
        if (await _repository.GetInProgressAsync() is not null)
            throw new ConflictException("C'è già un ordine in lavorazione, completalo prima di prenderne un altro.");

        var next = await _repository.GetNextInQueueAsync()
            ?? throw new NotFoundException("Non ci sono ordini in coda.");

        next.Status = OrderStatus.InProgress;
        next.StartedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(next);

        _logger.LogInformation("Ordine {Code}: stato aggiornato da {OldStatus} a {NewStatus}",
            next.Code, OrderStatus.Received, next.Status);

        return ToResponse(next);
    }

    public async Task<OrderResponse> CompleteCurrentAsync()
    {
        var current = await _repository.GetInProgressAsync()
            ?? throw new NotFoundException("Non c'è nessun ordine in lavorazione.");

        current.Status = OrderStatus.Completed;
        current.CompletedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(current);

        _logger.LogInformation("Ordine {Code}: stato aggiornato da {OldStatus} a {NewStatus}",
            current.Code, OrderStatus.InProgress, current.Status);

        return ToResponse(current);
    }

    private static string GenerateCode()
    {
        return "AP-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
    }

    private static OrderResponse ToResponse(Order order)
    {
        var items = order.Items
            .Select(i => new OrderItemResponse(i.PizzaName, i.Quantity, i.Notes))
            .ToList();

        return new OrderResponse(order.Code, order.CustomerName, order.Status.ToString(), items, order.CreatedAt);
    }
}
