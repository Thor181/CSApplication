using CSLibrary.V2.Data.Interfaces;

namespace CSLibrary.V2.Data.Models;

public partial class Place : IHelperEntity, IDbEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
