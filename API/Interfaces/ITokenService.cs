using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
  // it's going to create a toke and return it as a string
  Task<string> CreateToken(AppUser user);
  string GenerateRefreshToken();
}
