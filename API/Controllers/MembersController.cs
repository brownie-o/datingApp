using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Company.ClassLibrary1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize] // if not authorized, will get 401 Unauthorized response
    public class MembersController(IUnitOfWork uow, IPhotoService photoService) : BaseApiController
    {

        [HttpGet]
        // since pagingParams is an object, we need to tell the API controller to get it from the query string using [FromQuery]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery] MemberParams memberParams)
        {

            // get the current member id from the token claims and set it in the memberParams to exclude the current member from the results
            memberParams.CurrentMemberId = User.GetMemberId();

            return Ok(await uow.MemberRepository.GetMembersAsync(memberParams));
        }

        [HttpGet("{id}")] // localhost:5000/api/members/bob-id
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await uow.MemberRepository.GetMemberByIdAsync(id);

            if (member == null) return NotFound();

            return member;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForMember(string id)
        {
            var isCurrentUser = id == User.GetMemberId();
            return Ok(await uow.MemberRepository.GetPhotosForMemberAsync(id, isCurrentUser));
        }

        // Task<ActionResult>: not returning any specific data, just an HTTP response
        // no params: able to get the member id from the token
        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            // User is a property of ControllerBase (the base class of BaseApiController)
            // User is a ClaimsPrincipal, represents the current authenticated user. Contains One or more ClaimsIdentity
            // var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ClaimTypes.NameIdentifier is the user id stored in the token

            var memberId = User.GetMemberId();

            if (memberId == null) return BadRequest("Oops - no id found in token");

            var member = await uow.MemberRepository.GetMemberForUpdate(memberId);

            if (member == null) return BadRequest("Could not get member");

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

            uow.MemberRepository.Update(member); // optional: user can save with no changes without sending a bad request

            if (await uow.Complete()) return NoContent(); // NoContent() = 204 status code (success code for put requests)
            return BadRequest("Failed to update member");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file) // [FromForm] tell the API controller where to look for IFormFile
        {
            // User.GetMemberId(): extension method in API/Extensions/ClaimsPrincipalExtensions.cs
            // get the member id from the token claims and find the member for update
            var member = await uow.MemberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot update member");

            var result = await photoService.UploadPhotoAsync(file);

            // Error from Cloudinary
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.GetMemberId()
            };

            // if the user has no photos, make this the main photo
            // if (member.ImageUrl == null)
            // {
            //     member.ImageUrl = photo.Url;
            //     member.User.ImageUrl = photo.Url;
            // }

            member.Photos.Add(photo);

            if (await uow.Complete()) return photo;

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            // photos are included in GetMemberForUpdate
            var member = await uow.MemberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get member from token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

            if (member.ImageUrl == photo?.Url || photo == null)
            {
                return BadRequest("Cannot set this as main image");
            }

            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;

            if (await uow.Complete()) return NoContent();

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await uow.MemberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get member from token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

            if (photo == null || photo.Url == member.ImageUrl)
            {
                return BadRequest("This photo cannot be deleted");
            }

            // if a photo has a PublicId, it means it was uploaded to Cloudinary, and we need to delete it from there as well
            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            member.Photos.Remove(photo);
            if (await uow.Complete()) return Ok();

            return BadRequest("Problem deleting the photo");
        }
    }
}
