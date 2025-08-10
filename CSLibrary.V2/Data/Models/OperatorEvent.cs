using CSLibrary.V2.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.V2.Data.Models
{
    public class OperatorEvent : IDbEntity
    {
        public int Id { get; set; }
        public DateTime Dt { get; set; }
        public int TypeId { get; set; }
        public int PointId { get; set; }
        public virtual EventsType Type { get; set; } = null!;
        public virtual Point Point { get; set; } = null!;
    }
}
