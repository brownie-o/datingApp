using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using Company.ClassLibrary1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize] // if not authorized, will get 401 Unauthorized response
    public class MembersController(IMemberRepository memberRepository) : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            return Ok(await memberRepository.GetMembersAsync());
        }

        [HttpGet("{id}")] // localhost:5000/api/members/bob-id
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await memberRepository.GetMemberByIdAsync(id);

            if (member == null) return NotFound();

            return member;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForMember(string id)
        {
            return Ok(await memberRepository.GetPhotosForMemberAsync(id));
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

            if(memberId == null) return BadRequest("Oops - no id found in token");

            var member = await memberRepository.GetMemberForUpdate(memberId);

            if(member == null) return BadRequest("Could not get member");

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

            memberRepository.Update(member); // optional: user can save with no changes without sending a bad request

            if(await memberRepository.SaveAllAsync()) return NoContent(); // NoContent() = 204 status code (success code for put requests)
            return BadRequest("Failed to update member");
        }
    }
}
