using Core.Entities;

namespace Application.DTOs.CardDto;

public class CartDto
{
    public Guid UserId { get; set; }
    public List<CartItemsDto> Items { get; set; } = new List<CartItemsDto>();
}