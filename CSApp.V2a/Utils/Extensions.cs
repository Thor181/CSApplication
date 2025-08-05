using Avalonia.Rendering.Composition.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSApp.V2a.Utils
{
    public static class Extensions
    {
        extension(DateTime dateTime)
        {
            public DateOnly DateOnly => new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            public TimeOnly TimeOnly => new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }
}
