using FiguraSp.SharedLibrary.DependencyInjection;
using FiguraSp.Users.Api.Configuration;
using FiguraSp.Users.Model.Data;
using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }

        public static IServiceCollection AddJwtConfigurationAndValidation(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtConfiguration>(config.GetSection("Jwt"));
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters.ValidIssuer = config["Jwt:Issuer"];
                    options.TokenValidationParameters.ValidAudience = config["Jwt:Audience"];
                    options.TokenValidationParameters.IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!));
                });

            return services;
        }

        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthorization(options =>
            {
                var claim = config["Claim:DefaultClaim"]!;
                var key = config["Claim:RequiredKey"]!;
                options.AddPolicy(claim, policy => policy.RequireClaim(key));
            });

            return services;
        }
    }
}
