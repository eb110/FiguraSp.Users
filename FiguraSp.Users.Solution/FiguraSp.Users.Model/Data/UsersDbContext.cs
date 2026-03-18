using FiguraSp.Users.Model.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiguraSp.Users.Model.Data
{
    public class UsersDbContext(DbContextOptions<UsersDbContext> options) : IdentityDbContext(options)
    {
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

            base.OnModelCreating(builder);
        }

        public virtual async Task<T> GetFirstOrDefaultAsync<T>(IQueryable<T> query)
        {
            var entity = await query.FirstOrDefaultAsync();

            return entity!;
        }

        public virtual async Task<List<T>> GetEntitiesToListAsync<T>(IQueryable<T> query)
        {
            var entities = await query.ToListAsync();

            return entities;
        }
    }
}
