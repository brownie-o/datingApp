using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
  [HttpPost("register")] // api/account/register

  // passing parameters as params
  // public async Task<ActionResult<AppUser>> Register(string email, string displayName, string password)

  // passing as DTO (data transfer object) is safer
  public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
  {

    if (await EmailExists(registerDto.Email)) return BadRequest("Email taken");

    // HMACSHA512 is IDisposable and has a Dispose method that gets called at the end of the using block, adding using var ensures that the Dispose method is called automatically
    using var hmac = new HMACSHA512();

    var user = new AppUser
    {
      DisplayName = registerDto.DisplayName,
      Email = registerDto.Email,
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
      PasswordSalt = hmac.Key,
      Member = new Member
      {
        DisplayName = registerDto.DisplayName,
        Gender = registerDto.Gender,
        City = registerDto.City,
        Country = registerDto.Country, 
        DateOfBirth = registerDto.DateOfBirth
      }
    };

    context.Users.Add(user); // tells EF to track the user entity
    await context.SaveChangesAsync(); // saves the changes to the database

    return user.ToDto(tokenService);
  }

  [HttpPost("login")]
  public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
  {
    var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email);

    if (user == null) return Unauthorized("Invalid email address");

    using var hmac = new HMACSHA512(user.PasswordSalt);

    // compare the computed hash with the stored hash
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    for (var i = 0; i < computedHash.Length; i++)
    {
      if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
    }

    return user.ToDto(tokenService);
  }

  // check if Email exists
  private async Task<bool> EmailExists(string email)
  {
    // AnyAsync checks if any user in the Users DbSet has the same email (case insensitive)
    return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
  }
}
