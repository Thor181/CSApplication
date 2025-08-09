using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSourceGenerator
{
    public readonly record struct ConfigurationToGenerate
    {
        public readonly string Namespace;
        public readonly string Name;
        public readonly string SectionName;

        public ConfigurationToGenerate(string @namespace, string name, string sectionName)
        {
            Namespace = @namespace;
            Name = name;
            SectionName = sectionName;
        }
    }

    public readonly record struct SectionToGenerate
    {
        public readonly string SectionName;
        public readonly ImmutableArray<PropertyToGenerate> Propeties;

        public SectionToGenerate(string sectionName, ImmutableArray<PropertyToGenerate> propeties)
        {
            SectionName = sectionName;
            Propeties = propeties;
        }
    }

    public readonly record struct PropertyToGenerate
    {
        public readonly string Name;
        public readonly Type Type;

        public PropertyToGenerate(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
