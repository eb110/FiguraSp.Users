namespace FiguraSp.Users.Api.Configuration
{
    public record JwtConfiguration
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationInSeconds { get; set; }
        public string SecretKey { get; set; } = string.Empty;
    }
}
