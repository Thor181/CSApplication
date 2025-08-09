using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.V2
{
    public interface IPortWorkerOptions
    {
        public string PortInputName { get; set; }
        public string PortOutputName { get; set; }
        public string PortQR1Name { get; set; }
        public string PortQR2Name { get; set; }
        public string PointIdentifier { get; set; }
        public string[] FNNumbers { get; set; }
    }
}
