using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// properties bags
// hold properties so that we can pass them from a layer to anoter

public class RegisterDto
{
  [Required]
  public string DisplayName { get; set; } = "";
  [Required]
  [EmailAddress]
  public string Email { get; set; } = "";
  [Required]
  [MinLength(4)]
  public string Password { get; set; } = "";

  [Required] public string Gender { get; set; } = string.Empty;
  [Required] public string City { get; set; } = string.Empty;
  [Required] public string Country { get; set; } = string.Empty;
  [Required] public DateOnly DateOfBirth { get; set; }
}
