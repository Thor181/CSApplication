using CSLibrary.V2.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace CSLibrary.V2.Data.Models;

public partial class Qrevent : IDbEntity
{
    public Guid Id { get; set; }

    public DateTime Dt { get; set; }

    public int TypeId { get; set; }

    public int PointId { get; set; }

    public int PayId { get; set; }

    public decimal Sum { get; set; }

    public string Fn { get; set; } = null!;

    public string Fp { get; set; } = null!;

    public virtual PayType Pay { get; set; } = null!;

    public virtual Point Point { get; set; } = null!;

    public virtual EventsType Type { get; set; } = null!;
}
