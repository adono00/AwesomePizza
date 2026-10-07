using AwesomePizza.Service.Dtos;
using AwesomePizza.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AwesomePizza.Api.Controllers;

/// <summary>Endpoint per i clienti: non serve nessuna registrazione.</summary>
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Crea un nuovo ordine e restituisce il codice per seguirlo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        var order = await _orderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetByCode), new { code = order.Code }, order);
    }

    /// <summary>Restituisce lo stato di un ordine a partire dal suo codice.</summary>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        return Ok(await _orderService.GetByCodeAsync(code));
    }
}
