using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSApplication
{
    public class Render
    {
        public static async Task WriteLine(string line, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            await Console.Out.WriteLineAsync(line);
            Console.ResetColor();
        }
    }
}
