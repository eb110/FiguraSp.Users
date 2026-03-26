using FiguraSp.SharedLibrary.DependencyInjection;
using FiguraSp.Users.Model.Data;
using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Identity;

namespace FiguraSp.Users.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSharedDbConnection(this IServiceCollection services, IConfiguration config)
        {
            //add db
            SharedService.AddSharedSqlService<UsersDbContext>(services, config);

            return services;
        }

        public static IServiceCollection AddSharedJwtScheme(this IServiceCollection services, IConfiguration config)
        {
            //add db
            SharedService.AddJwtSharedService(services, config);

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

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }

        public static IServiceCollection AddSharedPolicyRules(this IServiceCollection services, IConfiguration config)
        {
            SharedService.AddSharedPolicyService(services, config);
            return services;
        }

        public static IApplicationBuilder UserSharedGatewayMiddleware(this IApplicationBuilder app)
        {
            //register middleware such as:
            //global exception -> handle external errors
            //listen to api gateway only -> block all outside calls
            SharedService.UseSharedGatewary(app);

            return app;
        }
    }
}
