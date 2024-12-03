using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utilities
{
    public static class TextUtilities
    {
        public static void ColorWrite(ConsoleColor color, string msg)
        {
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ResetColor();
        }
        public static void ColorWriteLine(ConsoleColor color, string msg) => ColorWrite(color, msg + "\n");
    }
}
