﻿using CSLibrary.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace CSLibrary.Data.Models;

public partial class EventsType : IHelperEntity, IDbEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<CardEvent> CardEvents { get; set; } = new List<CardEvent>();

    public virtual ICollection<Qrevent> Qrevents { get; set; } = new List<Qrevent>();
}
