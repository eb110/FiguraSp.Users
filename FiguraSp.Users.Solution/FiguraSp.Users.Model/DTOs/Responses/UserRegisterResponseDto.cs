using FiguraSp.SharedLibrary.Responses;

namespace FiguraSp.Users.Model.DTOs.Responses
{
    public record UserRegisterResponseDto : DefaultResponse
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}
