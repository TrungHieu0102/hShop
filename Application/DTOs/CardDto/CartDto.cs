using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CardDto;

public class CartDto
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<CartItemsDto> Items { get; set; } = new List<CartItemsDto>();
}