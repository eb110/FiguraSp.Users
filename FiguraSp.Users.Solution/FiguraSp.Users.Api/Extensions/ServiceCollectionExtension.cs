using FiguraSp.SharedLibrary.DependencyInjection;
using FiguraSp.Users.Model.Data;
using FiguraSp.Users.Service.Configuration;
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
            var key = Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!);
            var tokenValidationParams = new TokenValidationParameters
            {
                //encryption part of the token is added by us
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidIssuer = config["Jwt:Issuer"],
                ValidateIssuer = true,
                ValidAudience = config["Jwt:Audience"],
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
                //RequireExpirationTime = false
            };
            //singleton => its value stays the same for the entire life of the app
            services.AddSingleton(tokenValidationParams);

            services.Configure<JwtConfiguration>(config.GetSection("Jwt"));
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    //if the first fails
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenValidationParams;
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
