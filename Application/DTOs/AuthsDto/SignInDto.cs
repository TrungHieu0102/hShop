using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthsDto
{
    public class SignInDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "hatrunghieu121@gmail.com";
        [Required]
        public string Password { get; set; } = "113579hieM@";
    }
}
