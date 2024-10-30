namespace Application.DTOs.OrdersDto;

public class OrderDetailDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}