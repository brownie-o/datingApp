using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class Group(string name)
{
  [Key]
  public string Name { get; set; } = name;

  // navigation property to the connections in the group
  public ICollection<Connection> Connections { get; set; } = [];
}
