namespace AwesomePizza.Repository.Models;

public class OrderItem
{
    public string PizzaName { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public string Notes { get; set; } = "";
}
