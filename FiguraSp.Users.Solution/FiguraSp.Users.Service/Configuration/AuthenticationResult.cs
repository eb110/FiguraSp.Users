using FiguraSp.SharedLibrary.Responses;

namespace FiguraSp.Users.Service.Configuration
{
    public record AuthenticationResult : DefaultResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
