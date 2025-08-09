using CSLibrary.V2.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace CSLibrary.V2.Data.Models;

public partial class PayType : IDbEntity, IHelperEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Qrevent> Qrevents { get; set; } = new List<Qrevent>();
}
