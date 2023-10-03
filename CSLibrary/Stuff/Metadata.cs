using System;
using System.Linq;

namespace CSLibrary.Stuff
{
    public class Metadata
    {
        public static string GetClassLocalizedName(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(LocalizedNameAttribute), false).SingleOrDefault();

            if (attribute == null)
                return "Имя класса не локализовано";

            return ((LocalizedNameAttribute)attribute).LocalizedName;
        }
    }
}
