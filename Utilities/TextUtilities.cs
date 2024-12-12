using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utilities
{
    using C = Console;
    using CC = ConsoleColor;

    public static class TextUtilities
    {
        static Dictionary<string, CC> colorMap = new Dictionary<string, CC>
        {
            { "Whi", CC.White },
            { "Gra", CC.Gray },
            { "DGy", CC.DarkGray },
            { "Bla", CC.Black },

            { "Gre", CC.Green },
            { "DGe", CC.DarkGreen },

            { "Mgn", CC.Magenta },
            { "DMg", CC.DarkMagenta },

            { "Blu", CC.Blue },
            { "DBl", CC.DarkBlue },

            { "Red", CC.Red },
            { "DRe", CC.DarkRed },

            { "Yel", CC.Yellow },
            { "DYe", CC.DarkYellow },

            { "Cya", CC.Cyan },
            { "DCy", CC.DarkCyan }
        };
        static List<string> allColors = new List<string>
        {
            "Whi",
            "Gra",
            "DGy",
            "Bla",

            "Gre",
            "DGe",

            "Mgn",
            "DMg",

            "Blu",
            "DBl",

            "Red",
            "DRe",

            "Yel",
            "DYe",

            "Cya",
            "DCy"
        };

        private static Random rnd = new Random();

        public static string RandomColor()
        {
            int random;
            while ((random = rnd.Next(allColors.Count)) == 3);
            return GetColor(random);
        }
        public static string GetColor(int i)
        {
            if (i < 0 || i >= allColors.Count) return "";
            return ('@' + allColors[i]);
        }

        public static void ColorWrite(CC color, string msg)
        {
            C.ForegroundColor = color;
            C.Write(msg);
            C.ResetColor();
        }
        public static void ColorWriteLine(CC color, string msg) => ColorWrite(color, msg + "\n");

        /// <summary>
        /// Writes a string. It automatically changes text color if you include a substring: '@' followed by three letters. For example "@GreHi! I'm green." will print "Hi I'm green." with green text.
        /// </summary>
        /// <param name="msg">String with optional color substrings.</param>
        public static void CFW(string msg)
        {
            string substring = "";
            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == '@')
                {
                    if (substring.Length > 0) C.Write(substring);
                    substring = "";
                    i++;
                    string colorString = msg[i..(i+3)];
                    if (colorMap.TryGetValue(colorString, out CC color)) C.ForegroundColor = color;
                    i += 2;
                }
                else substring += msg[i];
            }
            C.Write(substring);
        }
        public static void CFWLine(string msg) => CFW(msg + "\n");

    }
}
