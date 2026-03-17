using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiguraSp.Users.Model.Entity
{
    public sealed record RefreshToken
    {
        public Guid Id { get; set; }
        public required string Token { get; set; }
        public required string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;
        public DateTime ExpiresOnUtc { get; set; }
    }
}
