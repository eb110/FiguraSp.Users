using FiguraSp.SharedLibrary.DependencyInjection;
using FiguraSp.Users.Model.Data;
using Microsoft.AspNetCore.Identity;

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

        //needed for controller and other stuff
        public static IServiceCollection AddIdentityUser(this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    //controller connection
                    .AddEntityFrameworkStores<UsersDbContext>();

            return services;
        }
    }
}
