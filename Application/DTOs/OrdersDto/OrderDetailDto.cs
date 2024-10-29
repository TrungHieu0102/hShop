namespace Application.DTOs.OrdersDto;

public class OrderDetailDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}