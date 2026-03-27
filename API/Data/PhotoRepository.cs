using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository(AppDbContext context) : IPhotoRepository
{
  public async Task<Photo?> GetPhotoById(int id)
  {
    // ignore query filters to get the photo even if it's not approved
    return await context.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == id);
  }

  public async Task<IReadOnlyList<PhotoForApprovalDto>> GetUnapprovedPhotos()
  {
    return await context.Photos.IgnoreQueryFilters().Where(x => x.IsApproved == false).Select(x => new PhotoForApprovalDto
    {
      Id = x.Id,
      UserId = x.MemberId,
      Url = x.Url,
      IsApproved = x.IsApproved
    }).ToListAsync();
  }

  public void RemovePhoto(Photo photo)
  {
    context.Photos.Remove(photo);
  }
}
