using FiguraSp.Users.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSharedServices(builder.Configuration)
    .AddIdentityUser()
    .AddUserService();

builder.Services.AddControllers();


var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
