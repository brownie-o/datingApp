using System;
using API.Entities;
using API.Helpers;
using Company.ClassLibrary1;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

// “I am a class that implements IMemberRepository.”
public class MemberRepository(AppDbContext context) : IMemberRepository
{
  public async Task<Member?> GetMemberByIdAsync(string id)
  {
    return await context.Members.FindAsync(id);
  }

  public async Task<Member?> GetMemberForUpdate(string id)
  {
    // Include: eager loading related entities
    // will include the User object & photos when getting the member for update
    return await context.Members.Include(x => x.User).Include(x => x.Photos).SingleOrDefaultAsync(x => x.Id == id);
  }

  public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
  {
    // AsQueryable(): set query as queryable to allow further querying
    var query = context.Members.AsQueryable();

    // exclude the current member from the results
    query = query.Where(x => x.Id != memberParams.CurrentMemberId);

    if (memberParams.Gender != null)
    {
      query = query.Where(x => x.Gender == memberParams.Gender);
    }

    var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
    var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));

    query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

    // 排列
    query = memberParams.OrderBy switch
    {
      "created" => query.OrderByDescending(x => x.Created),
      // _: default case, if OrderBy is not "created", order by lastActive
      _ => query.OrderByDescending(x => x.LastActive)
    };

    return await PaginationHelper.CreateAsync(query, memberParams.PageNumber, memberParams.PageSize);

    // return await context.Members
    //   .Include(x => x.Photos) // if eager loading photos along with members
    //   .ToListAsync();
  }

  public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
  {
    return await context.Members.Where(x => x.Id == memberId).SelectMany(x => x.Photos).ToListAsync();
    // return a readonly list of photos for a specific member
  }

  // public async Task<bool> SaveAllAsync()
  // {
  //   // SaveChangesAsync(): Saves all changes made in this context to the database.
  //   return await context.SaveChangesAsync() > 0;
  //   // The task result contains the number of state entries written to the database.
  // }

  // use this method to save the same thing to the database
  public void Update(Member member)
  {
    // updating the tracking of the entity to say something has been modified
    context.Entry(member).State = EntityState.Modified;
  }
}
