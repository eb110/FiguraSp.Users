using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiguraSp.Users.Model.Data
{
    public class UsersDbContext(DbContextOptions<UsersDbContext> options) : IdentityDbContext(options)
    {
    }
}
