using FiguraSp.SharedLibrary.Responses;

namespace FiguraSp.Users.Model.DTOs.Responses
{
    public record ClaimResponseDto : DefaultResponse
    {
        public required string ClaimKey { get; set; }
        public required string ClaimValue { get; set; }
    }
}
