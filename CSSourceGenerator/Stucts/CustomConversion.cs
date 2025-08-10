using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSourceGenerator.Stucts
{
    public readonly record struct CustomConversion
    {
        public readonly string PropertyName;
        public readonly Type Type;

        public CustomConversion(string propertyName, Type type)
        {
            PropertyName = propertyName;
            Type = type;
        }

        public static IEnumerable<CustomConversion> FromStringArray(IEnumerable<string> array)
        {
            foreach (var item in array)
            {
                var parts = item.Split(':');
                yield return new CustomConversion(parts[0], Type.GetType(parts[1]));
            }
        }
    }
}
