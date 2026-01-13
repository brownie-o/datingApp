using System.Text;
using API.Data;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
  opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// to solve CORS issues
builder.Services.AddCors();
// “When someone asks for ITokenService, create a TokenService instance using this lifetime rule.”
builder.Services.AddScoped<ITokenService, TokenService>();
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
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// error-handle middleware - must be placed at the top to catch errors from all other middlewares
app.UseMiddleware<ExceptionMiddleware>();

// CORS policy
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200"));

// Authentication: who are you
app.UseAuthentication();
// Authorization: are they allowed
app.UseAuthorization();

app.MapControllers();

app.Run();
