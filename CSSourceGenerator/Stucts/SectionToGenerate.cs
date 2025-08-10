using System.Collections.Immutable;

namespace CSSourceGenerator.Stucts
{
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
}
