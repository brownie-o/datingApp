using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
{
  [HttpPost("register")] // api/account/register

  // passing parameters as params
  // public async Task<ActionResult<AppUser>> Register(string email, string displayName, string password)

  // passing as DTO (data transfer object) is safer
  public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
  {

    // if (await EmailExists(registerDto.Email)) return BadRequest("Email taken"); => enforcing unique email using Identity

    // HMACSHA512 is IDisposable and has a Dispose method that gets called at the end of the using block, adding using var ensures that the Dispose method is called automatically
    // using var hmac = new HMACSHA512(); => Identity is responsible for hashing and salting the password

    var user = new AppUser
    {
      DisplayName = registerDto.DisplayName,
      Email = registerDto.Email,
      UserName = registerDto.Email,
      // PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
      // PasswordSalt = hmac.Key,
      Member = new Member
      {
        DisplayName = registerDto.DisplayName,
        Gender = registerDto.Gender,
        City = registerDto.City,
        Country = registerDto.Country,
        DateOfBirth = registerDto.DateOfBirth
      }
    };

    // context.Users.Add(user); // tells EF to track the user entity
    // await context.SaveChangesAsync(); // saves the changes to the database

    var result = await userManager.CreateAsync(user, registerDto.Password);
    if (!result.Succeeded)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError("identity", error.Description);
      }
      return ValidationProblem();
    }

    await userManager.AddToRoleAsync(user, "Member");

    await SetRefreshTokenCookie(user);

    return await user.ToDto(tokenService);
  }

  [HttpPost("login")]
  public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
  {
    // var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email);
    var user = await userManager.FindByEmailAsync(loginDto.Email);

    if (user == null) return Unauthorized("Invalid email address");

    // using var hmac = new HMACSHA512(user.PasswordSalt);

    // // compare the computed hash with the stored hash
    // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    // for (var i = 0; i < computedHash.Length; i++)
    // {
    //   if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
    // }


    // Identity handles the password verification, so we can use the CheckPasswordAsync method to verify the password
    var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

    if (!result) return Unauthorized("Invalid password");

    await SetRefreshTokenCookie(user);

    return await user.ToDto(tokenService);
  }

  [HttpPost("refresh-token")]
  public async Task<ActionResult<UserDto>> RefreshToken()
  {
    var refreshToken = Request.Cookies["refreshToken"];

    if (refreshToken == null) return NoContent();

    var user = await userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiry > DateTime.UtcNow);

    if (user == null) return Unauthorized();

    await SetRefreshTokenCookie(user);

    return await user.ToDto(tokenService);
  }

  // check if Email exists
  // private async Task<bool> EmailExists(string email)
  // {
  //   // AnyAsync checks if any user in the Users DbSet has the same email (case insensitive)
  //   return await context.Users.AnyAsync(x => x.Email!.ToLower() == email.ToLower());
  // }

  private async Task SetRefreshTokenCookie(AppUser user)
  {
    var refreshToken = tokenService.GenerateRefreshToken();
    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
    await userManager.UpdateAsync(user);

    var cookieOptions = new CookieOptions
    {
      HttpOnly = true, // not accessible to JavaScript
      Secure = true, // only sent over HTTPS
      SameSite = SameSiteMode.Strict, // not sent with cross-site requests
      Expires = DateTime.UtcNow.AddDays(7) // expires in 7 days
    };

    Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
  }

  [Authorize]
  [HttpPost("logout")]
  public async Task<ActionResult> Logout()
  {
    await userManager.Users
      .Where(x => x.Id == User.GetMemberId())
      .ExecuteUpdateAsync(setters => setters
        .SetProperty(x => x.RefreshToken, _ => null)
        .SetProperty(x => x.RefreshTokenExpiry, _ => null)
      );

    Response.Cookies.Delete("refreshToken");

    return Ok();
  }
}
