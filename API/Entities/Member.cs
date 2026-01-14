using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Member
{
  public string Id { get; set; } = null!; // Assuming Id is assigned elsewhere, set initially to null and ! to avoid warnings
  public DateOnly DateOfBirth { get; set; }
  public string? ImageUrl { get; set; }
  public required string DisplayName { get; set; }
  public DateTime Created { get; set; } = DateTime.UtcNow; // browser will transfer to local time
  public DateTime LastActive { get; set; } = DateTime.UtcNow;
  public required string Gender { get; set; }
  public string? Description { get; set; }
  public required string City { get; set; }
  public required string Country { get; set; }


  // Navigation property, will be able to navigate from Member Object to the User

  // JsonIgnore: the list of photos does not go back with the member object when we serialize it to JSON
  [JsonIgnore]
  public List<Photo> Photos { get; set; } = [];

  // The User navigation property (the AppUser object) is related to this Member through the Id foreign key
  // The Member.Id references AppUser.Id (establishes the relationship)
  // It creates a one-to-one relationship: one Member belongs to one AppUser
  [JsonIgnore]
  [ForeignKey(nameof(Id))] // Id: the foreign key property of the Member class
  public AppUser User { get; set; } = null!;
}
