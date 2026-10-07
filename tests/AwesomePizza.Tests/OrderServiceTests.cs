using AwesomePizza.Service.Dtos;
using AwesomePizza.Service.Exceptions;
using AwesomePizza.Service.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AwesomePizza.Tests;

public class OrderServiceTests
{
    private readonly FakeOrderRepository _repository = new();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(_repository, NullLogger<OrderService>.Instance);
    }

    private static CreateOrderRequest NewRequest(string customer = "Mario") => new()
    {
        CustomerName = customer,
        Items = new() { new OrderItemRequest { PizzaName = "Margherita", Quantity = 2 } }
    };

    private static CreateOrderRequest NewRequestWithNotes(string customer = "Mario", string notes = "No cipolla") => new()
    {
        CustomerName = customer,
        Items = new() { new OrderItemRequest { PizzaName = "Margherita", Quantity = 2, Notes = notes } }
    };

    [Fact]
    public async Task Create_ReturnsCodeAndReceivedStatus()
    {
        var order = await _service.CreateAsync(NewRequest());

        Assert.StartsWith("AP-", order.Code);
        Assert.Equal("Received", order.Status);
        Assert.Single(order.Items);
    }

    [Fact]
    public async Task Create_ReturnsCodeAndReceivedStatusAndNotes()
    {
        var order = await _service.CreateAsync(NewRequestWithNotes());

        Assert.StartsWith("AP-", order.Code);
        Assert.Equal("Received", order.Status);
        Assert.Single(order.Items);
        Assert.Equal("No cipolla", order.Items[0].Notes);
    }

    [Fact]
    public async Task GetByCode_ReturnsTheOrder()
    {
        var created = await _service.CreateAsync(NewRequest("Luigi"));

        var found = await _service.GetByCodeAsync(created.Code);

        Assert.Equal("Luigi", found.CustomerName);
    }

    [Fact]
    public async Task GetByCode_UnknownCode_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByCodeAsync("AP-XXXXXX"));
    }

    [Fact]
    public async Task StartNext_TakesTheOldestOrder()
    {
        var first = await _service.CreateAsync(NewRequest("Primo"));
        await _service.CreateAsync(NewRequest("Secondo"));

        var started = await _service.StartNextAsync();

        Assert.Equal(first.Code, started.Code);
        Assert.Equal("InProgress", started.Status);
    }

    [Fact]
    public async Task StartNext_WithAnOrderInProgress_ThrowsConflict()
    {
        await _service.CreateAsync(NewRequest());
        await _service.CreateAsync(NewRequest());
        await _service.StartNextAsync();

        await Assert.ThrowsAsync<ConflictException>(() => _service.StartNextAsync());
    }

    [Fact]
    public async Task CheckCurrent_WithOrderInProgress()
    {

        await _service.CreateAsync(NewRequest());
        await _service.StartNextAsync();
        var order = await _service.GetCurrentAsync();

        Assert.Equal("InProgress", order.Status);
    }

    [Fact]
    public async Task CheckCurrent_WithOrderNoProgress_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCurrentAsync());
    }

    [Fact]
    public async Task StartNext_WithEmptyQueue_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.StartNextAsync());
    }

    [Fact]
    public async Task CompleteCurrent_MarksTheOrderAsCompleted()
    {
        var created = await _service.CreateAsync(NewRequest());
        await _service.StartNextAsync();

        var completed = await _service.CompleteCurrentAsync();

        Assert.Equal(created.Code, completed.Code);
        Assert.Equal("Completed", completed.Status);
    }

    [Fact]
    public async Task CompleteCurrent_WithNothingInProgress_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CompleteCurrentAsync());
    }

    [Fact]
    public async Task Queue_ContainsOnlyOrdersNotYetStarted()
    {
        await _service.CreateAsync(NewRequest("Primo"));
        await _service.CreateAsync(NewRequest("Secondo"));
        await _service.StartNextAsync();

        var queue = await _service.GetQueueAsync();

        Assert.Single(queue);
        Assert.Equal("Secondo", queue[0].CustomerName);
    }
}
