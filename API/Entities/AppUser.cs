using Microsoft.AspNetCore.Identity;

namespace API.Entities;

// Database Model, represents the Users table in database
// Should never be sent directly to the frontend, currently sending UserDto (in AppUserExtensions) instead
public class AppUser : IdentityUser
{
  public required string DisplayName { get; set; }
  public string? ImageUrl { get; set; }
  public string? RefreshToken { get; set; }
  public DateTime? RefreshTokenExpiry { get; set; }

  // navigation property
  public Member Member { get; set; } = null!;
}
