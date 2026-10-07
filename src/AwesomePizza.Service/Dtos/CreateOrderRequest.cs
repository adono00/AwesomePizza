using System.ComponentModel.DataAnnotations;

namespace AwesomePizza.Service.Dtos;

public class CreateOrderRequest
{
    [Required]
    public string CustomerName { get; set; } = "";

    [Required]
    [MinLength(1)]
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    [Required]
    public string PizzaName { get; set; } = "";

    [Range(1, 20)]
    public int Quantity { get; set; } = 1;
    [StringLength(200)]
    public string Notes { get; set; } = "";
}
