using System;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
  Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId); // sourceMemberId, targetMemberId is the id of the entity
  Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams);
  Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
  void DeleteLike(MemberLike like);
  void AddLike(MemberLike like);
}
