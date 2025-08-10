using System;

namespace CSSourceGenerator.Stucts
{
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
