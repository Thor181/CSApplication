using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.Stuff
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LocalizedNameAttribute : Attribute
    {
        public string LocalizedName { get; private set; }

        public LocalizedNameAttribute(string localizedName)
        {
            LocalizedName = localizedName;
        }
    }
}
