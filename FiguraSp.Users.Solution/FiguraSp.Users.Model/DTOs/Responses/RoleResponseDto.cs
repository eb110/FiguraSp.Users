using FiguraSp.SharedLibrary.Responses;

namespace FiguraSp.Users.Model.DTOs.Responses
{
    public record RoleResponseDto : DefaultResponse
    {
        public string? RoleName { get; set; }
        public string? UserName { get; set; }
    }
}
