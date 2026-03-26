using System.Text;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SignalR;
using Company.ClassLibrary1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container. Registering services

// AddControllers: add the controllers to the dependency injection container, so that they can be used in the application
builder.Services.AddControllers();

// AddDbContext: add the database context to the dependency injection container, so that it can be used in the application
builder.Services.AddDbContext<AppDbContext>(opt =>
{
  // configuring how the DbContext connects to the database:
  // use SQLite as the database provider, and get the connection string from the configuration (appsettings.json)
  opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// to solve CORS issues
builder.Services.AddCors();
// “ When someone asks for ITokenService, create a TokenService instance using this lifetime rule. ”
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<LogUserActivity>();
// builder.Configuration.GetSection("CloudinarySettings"): get the CloudinarySettings section from appsettings.json
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>(); // AddSingleton: will not get destroyed until the application is stopped

builder.Services.AddIdentityCore<AppUser>(opt =>
{
  opt.Password.RequireNonAlphanumeric = false; // password does not require non-alphanumeric characters
  opt.User.RequireUniqueEmail = true; // email must be unique
}).AddRoles<IdentityRole>() // covering role base authentication, using IdentityRole Type
.AddEntityFrameworkStores<AppDbContext>(); // using entity framework to store user data

// Looks for Authorization header, Checks Bearer <token>
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
  var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("TokenKey not found - Program.cs");
  options.TokenValidationParameters = new TokenValidationParameters
  {
    // specify how we want to validate the token
    ValidateIssuerSigningKey = true, // this token is valid while api receives it
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
    ValidateIssuer = false, // ValidateIssuer: does not accept tokens issued by certain issuer
    ValidateAudience = false // ValidateAudience: specify who is the audience of the token
  };

// real time communication
// hook into the JwtBearerEvents, get hold of the token from the query string, and if the request is for our hubs, then we will use that token to authenticate the user
  options.Events = new JwtBearerEvents
  {
    // context: HttpContext
    OnMessageReceived = context =>
    {
      var accessToken = context.Request.Query["access_token"];
      
      var path = context.HttpContext.Request.Path;
      if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
      {
        context.Token = accessToken;
      }
      return Task.CompletedTask;
    }
  };
});

builder.Services.AddAuthorizationBuilder()
  .AddPolicy("RequiredAdminRole", policy => policy.RequireRole("Admin"))
  .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

var app = builder.Build();

// 2. Configure the HTTP request pipeline. (middlewares that handle the HTTP requests)

// error-handle middleware - must be placed at the top to catch errors from all other middlewares
app.UseMiddleware<ExceptionMiddleware>();

// CORS policy
// AllowCredentials: allow the client to send cookies to the API, and allow the API to set cookies in the client
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200", "https://localhost:4200"));

// Authentication: who are you
app.UseAuthentication(); // => create User object on the HttpContext, ControllerBase can use it
// Authorization: are they allowed
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence"); // map the PresenceHub to the /hubs/presence endpoint
app.MapHub<MessageHub>("hubs/messages");

// seed data into datatbase
using var scpoe = app.Services.CreateScope();
var services = scpoe.ServiceProvider;
try
{
  var context = services.GetRequiredService<AppDbContext>(); // get the database context
  var userManager = services.GetRequiredService<UserManager<AppUser>>();
  await context.Database.MigrateAsync(); // apply any pending migrations
  await context.Connections.ExecuteDeleteAsync(); // empty the connections table when the application starts
  await Seed.SeedUsers(userManager); // since we used static in Seed.cs, we can call SeedUsers directly
}
catch (Exception ex)
{
  var logger = services.GetRequiredService<ILogger<Program>>();
  logger.LogError(ex, "An error occurred during migration");
}

// 3. Run the application
app.Run();
