using AwesomePizza.Service;
using AwesomePizza.Service.Dtos;
using AwesomePizza.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwesomePizza.Api.Controllers;

/// <summary>Endpoint per il pizzaiolo (richiedono il login).</summary>
[ApiController]
[Route("api/kitchen")]
[Authorize(Roles = Roles.Pizzaiolo)]
public class KitchenController : ControllerBase
{
    private readonly IOrderService _orderService;

    public KitchenController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Elenca gli ordini in attesa, dal più vecchio al più recente.</summary>
    [HttpGet("queue")]
    [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQueue()
    {
        return Ok(await _orderService.GetQueueAsync());
    }

    /// <summary>Mostra l'ordine attualmente in preparazione.</summary>
    [HttpGet("current")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrent()
    {
        return Ok(await _orderService.GetCurrentAsync());
    }

    /// <summary>Prende in carico il prossimo ordine in coda.</summary>
    /// <remarks>Si può avere un solo ordine in lavorazione alla volta.</remarks>
    [HttpPost("start-next")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StartNext()
    {
        return Ok(await _orderService.StartNextAsync());
    }

    /// <summary>Segna come completato l'ordine attualmente in lavorazione.</summary>
    [HttpPost("complete-current")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteCurrent()
    {
        return Ok(await _orderService.CompleteCurrentAsync());
    }
}