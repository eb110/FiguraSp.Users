using FiguraSp.SharedLibrary.Responses;

namespace FiguraSp.Users.Service.Configuration
{
    public record AuthenticationResult : DefaultResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
