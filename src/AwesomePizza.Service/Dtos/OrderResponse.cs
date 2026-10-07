namespace AwesomePizza.Service.Dtos;

/// <summary>Stato di un ordine. Status: Received, InProgress oppure Completed.</summary>
public record OrderResponse(
    string Code,
    string CustomerName,
    string Status,
    List<OrderItemResponse> Items,
    DateTime CreatedAt);

public record OrderItemResponse(string PizzaName, int Quantity, string Notes);
