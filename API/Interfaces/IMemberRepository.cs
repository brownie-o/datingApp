using System;
using API.Entities;

namespace Company.ClassLibrary1;

// interface for MemberRepository
// “Any class that claims to be an IMemberRepository MUST provide these methods.”
public interface IMemberRepository
{
  // not returning anything, thus using void
  void Update(Member member);
  Task<bool> SaveAllAsync(); // for saving changes
  Task<IReadOnlyList<Member>> GetMembersAsync();
  Task<Member?> GetMemberByIdAsync(string id);
  Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);

  // get the member to update
  Task<Member?> GetMemberForUpdate(string id);
}
