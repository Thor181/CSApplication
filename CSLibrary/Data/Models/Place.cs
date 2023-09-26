using System;
using System.Collections.Generic;

namespace CSLibrary.Data.Models;

public partial class Place
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
