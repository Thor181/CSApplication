using System;
using System.Collections.Generic;

namespace CSLibrary.Data.Models;

public partial class CardEvent
{
    public int Id { get; set; }

    public DateTime Dt { get; set; }

    public int TypeId { get; set; }

    public int PointId { get; set; }

    public string Card { get; set; } = null!;

    public virtual Point Point { get; set; } = null!;

    public virtual EventsType Type { get; set; } = null!;
}
