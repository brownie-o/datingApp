using System;
using System.Security.Claims;

namespace API.Extensions;

// extension method so should be static class
public static class ClaimsPrincipalExtensions
{
  // will return the member id string from the ClaimsPrincipal
  public static string GetMemberId(this ClaimsPrincipal user)
  {
    // FindFirstValue: retrieves the value of the first claim with the specified claim type
    // ClaimTypes.NameIdentifier: standard claim type representing the unique identifier of the user
    // getting the member id from the token claims
    return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get memberId from token");
  }
}
