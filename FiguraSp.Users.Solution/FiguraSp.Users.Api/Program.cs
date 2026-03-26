using FiguraSp.Users.Api.Extensions;
using FiguraSp.Users.Service.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentityUser();
builder.Services.AddSharedDbConnection(builder.Configuration);

builder.Services.AddCustomServices();
builder.Services.AddSharedJwtScheme(builder.Configuration);
builder.Services.AddSharedPolicyRules(builder.Configuration);

//options access for JwtConfiguration in the controller
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));


//##############################################################MIDDLEWARE###############

var app = builder.Build();
app.UserSharedGatewayMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
