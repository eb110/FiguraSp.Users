using System.ComponentModel.DataAnnotations;

namespace FiguraSp.Users.Model.DTOs.Requests
{
    public class TokenRequestDto
    {
        [Required]
        public required string Token { get; set; }
        [Required]
        public required string RefreshToken { get; set; }
    }
}
