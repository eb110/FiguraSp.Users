using System.ComponentModel.DataAnnotations;

namespace FiguraSp.Users.Model.DTOs.Requests
{
    public class UserRegistrationRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}
