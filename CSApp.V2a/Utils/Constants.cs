using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSApp.V2a.Utils
{
    public static class Constants
    {
        public static class Paths
        {
            public readonly static string MainFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MfRA", "CSApplication", "CSAppV2");
        }

        public const string AppName = "CSApp.V2a";
        
    }
}
