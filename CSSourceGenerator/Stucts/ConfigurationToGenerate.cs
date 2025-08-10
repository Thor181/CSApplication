using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSourceGenerator.Stucts
{
    public readonly record struct ConfigurationToGenerate
    {
        public readonly string Namespace;
        public readonly string Name;
        public readonly string SectionName;
        public readonly CustomConversion[] CustomConversion;

        public ConfigurationToGenerate(string @namespace, string name, string sectionName, CustomConversion[] customConversion)
        {
            Namespace = @namespace;
            Name = name;
            SectionName = sectionName;
            CustomConversion = customConversion;
        }
    }
}
