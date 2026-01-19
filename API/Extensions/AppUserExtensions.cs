using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

// static class: do not need to create an instance of the class to access its methods
public static class AppUserExtensions
{
  // converts the database AppUser entity into a UserDto object that gets sent to the frontend.
  // Represents what gets sent to the frontend
  public static UserDto ToDto(this AppUser user, ITokenService tokenService)
  {
    return new UserDto
    {
      Id = user.Id,
      DisplayName = user.DisplayName,
      Email = user.Email,
      ImageUrl = user.ImageUrl,
      Token = tokenService.CreateToken(user)
    };
  }
}
