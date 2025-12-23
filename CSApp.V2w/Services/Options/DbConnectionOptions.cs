using CSApp.V2.Generated.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSApp.V2w.Services.Options
{
    [Configuration(Section)]
    public partial class DbConnectionOptions
    {
        public const string Section = "DbConnection";
    } 
}
