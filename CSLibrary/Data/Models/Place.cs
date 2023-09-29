using CSLibrary.Data.Interfaces;

namespace CSLibrary.Data.Models;

public partial class Place : IHelperEntity, IDbEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
