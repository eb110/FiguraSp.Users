using FiguraSp.SharedLibrary.DependencyInjection;
using FiguraSp.Users.Model.Data;

namespace FiguraSp.Users.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration config)
        {
            //add db
            SharedService.AddSharedServies<UsersDbContext>(services, config);

            return services;
        }
    }
}
