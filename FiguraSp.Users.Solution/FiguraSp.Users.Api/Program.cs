using FiguraSp.Users.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSharedServices(builder.Configuration)
    .AddIdentityUser()
    .AddUserService()
    .AddJwtConfigurationAndValidation(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddControllers();


//##############################################################MIDDLEWARE###############

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
