using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// : = class inheritance
//  : BaseApiController = BuggyController inherits from the BaseApiController class
public class BuggyController : BaseApiController
{
  [HttpGet("auth")]
  public IActionResult GetAuth()
  {
    return Unauthorized();
  }

  [HttpGet("Not-Found")]
  public IActionResult GetNotFound()
  {
    return NotFound();
  }

  [HttpGet("server-error")]
  public IActionResult GetServerError()
  {
    throw new Exception("This is a server error");
  }

  [HttpGet("bad-request")]
  public IActionResult GetBadRequest()
  {
    return BadRequest("This is not a good request");
  }
}
