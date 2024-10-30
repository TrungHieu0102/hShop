using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace Application.DTOs.OrdersDto;

public class UpdateStatusDto<T> where T : Enum
{
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    public T Status { get; set; }
}