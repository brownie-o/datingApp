using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
  // seed data into database
  // SeedUsers is running while starting the application
  public static async Task SeedUsers(AppDbContext context)
  {
    if (await context.Users.AnyAsync()) return;

    // will have access to UserSeedData.json
    var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
    // deserialize JSON data to C# objects(into a List of Member)
    var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

    if (members == null)
    {
      Console.WriteLine("No members in seed data");
      return;
    }

    // for password hashing

    foreach (var member in members)
    {
      using var hmac = new HMACSHA512();
      var user = new AppUser
      {
        Id = member.Id,
        Email = member.Email,
        DisplayName = member.DisplayName,
        ImageUrl = member.ImageUrl,
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
        PasswordSalt = hmac.Key,
        Member = new Member
        {
          Id = member.Id,
          DisplayName = member.DisplayName,
          Description = member.Description,
          DateOfBirth = member.DateOfBirth,
          ImageUrl = member.ImageUrl,
          Gender = member.Gender,
          City = member.City,
          Country = member.Country,
          LastActive = member.LastActive,
          Created = member.Created
        }
      };

      // Photos: Navigation property from Member.cs
      user.Member.Photos.Add(new Photo
      {
        Url = member.ImageUrl!,
        MemberId = member.Id,
      });

      // track the user entity to be added to the database
      context.Users.Add(user);
    }

    await context.SaveChangesAsync();
  }
}
