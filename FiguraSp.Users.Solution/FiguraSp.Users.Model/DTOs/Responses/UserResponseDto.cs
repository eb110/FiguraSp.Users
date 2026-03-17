using FiguraSp.SharedLibrary.Responses;

namespace FiguraSp.Users.Model.DTOs.Responses
{
    public record UserResponseDto : DefaultResponse
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

    }
}
