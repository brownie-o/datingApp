using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService
{
  public async Task<string> CreateToken(AppUser user)
  {
    var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot get token key");
    if (tokenKey.Length < 64) throw new Exception("Your token key must be at least 64 characters long");
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

    // information about the users
    var claims = new List<Claim>
    {
      new(ClaimTypes.Email, user.Email!),
      new(ClaimTypes.NameIdentifier, user.Id)
    };

    // get the roles for the user and add them to the claims
    var roles = await userManager.GetRolesAsync(user);

    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // HmacSha512Signature: what we use to sign the token

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      // Expires = DateTime.UtcNow.AddMinutes(7), // For production, set shorter expiration time and use refresh tokens to get new access tokens. 
      Expires = DateTime.UtcNow.AddDays(15), // For development, set longer expiration time to avoid having to log in frequently and use free SQL server
      SigningCredentials = creds
    };

    var tokenHandler = new JwtSecurityTokenHandler(); // class where we create the token
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
  }

  public string GenerateRefreshToken()
  {
    var randomBytes = RandomNumberGenerator.GetBytes(64);
    return Convert.ToBase64String(randomBytes);
  }
}
