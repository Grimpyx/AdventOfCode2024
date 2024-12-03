using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utilities
{
    public enum DayDataType
    {
        Simple,     // simple data
        Full        // full data
    }

    public static class DataLoader
    {

        public static string LoadAllData(int day, DayDataType type = DayDataType.Full)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DaysData");

            switch (type)
            {
                case DayDataType.Simple:
                    path = Path.Combine(path, "Day" + day + "Simple.txt");
                    break;
                case DayDataType.Full:
                    path = Path.Combine(path, "Day" + day + ".txt");
                    break;
                default:
                    break;
            }

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to load row data: {0}", e.ToString());
                Console.ResetColor();
                return "ERROR";
            }
        }

        public static string[] LoadRowData(int day, DayDataType type = DayDataType.Full)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DaysData");

            switch (type)
            {
                case DayDataType.Simple:
                    path = Path.Combine(path, "Day" + day + "Simple.txt");
                    break;
                case DayDataType.Full:
                    path = Path.Combine(path, "Day" + day + ".txt");
                    break;
                default:
                    break;
            }

            List<string> rows = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        string? line = sr.ReadLine();
                        if (line == null) break;
                        rows.Add(line);
                    }
                }
                return rows.ToArray();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to load row data: {0}", e.ToString());
                Console.ResetColor();
                return ["ERROR"];
            }
        }
    }
}
