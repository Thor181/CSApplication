using CSApp.V2a.Generated.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSApp.V2a.Services.Options
{
    [Configuration(Section)]
    public partial class DbConnectionOptions
    {
        public const string Section = "DbConnection";
    } 
}
