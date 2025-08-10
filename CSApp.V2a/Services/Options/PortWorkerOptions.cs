using CSApp.V2a.Generated.Configuration;
using CSLibrary.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSApp.V2a.Services.Options
{

    [Configuration(Section, [ "N1:System.Decimal", "N2:System.Decimal" ])]
    public partial class PortWorkerOptions : IPortWorkerOptions
    { 
        public const string Section = "PortWorker"; 
    } 
}
