using System;
using System.Security.Claims;

namespace API.Extensions;

// extension method so should be static class
public static class ClaimsPrincipalExtensions
{
  // will return the member id string from the ClaimsPrincipal
  public static string GetMemberId(this ClaimsPrincipal user)
  {
    return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get memberId from token");
  }
}
