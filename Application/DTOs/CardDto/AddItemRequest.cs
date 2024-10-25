namespace Application.DTOs.CardDto;

public class AddItemRequest
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}