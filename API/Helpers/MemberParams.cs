using System;

namespace API.Helpers;

// MemberParams includes all of the PagingParams properties
public class MemberParams : PagingParams
{
  public string? Gender { get; set; }
  public string? CurrentMemberId { get; set; }

  public int MinAge { get; set; } = 18;
  public int MaxAge { get; set; } = 100;

  public string OrderBy { get; set; } = "lastActive";
}
