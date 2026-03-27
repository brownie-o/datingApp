using System;
using API.Entities;
using API.Helpers;

namespace Company.ClassLibrary1;

// interface for MemberRepository
// “Any class that claims to be an IMemberRepository MUST provide these methods.”
public interface IMemberRepository
{
  // not returning anything, thus using void
  void Update(Member member);
  Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);
  Task<Member?> GetMemberByIdAsync(string id);
  Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId, bool isCurentUser);

  // get the member to update
  Task<Member?> GetMemberForUpdate(string id);
}
