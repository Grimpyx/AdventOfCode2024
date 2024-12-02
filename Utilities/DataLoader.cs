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

        public static string LoadData(int day, DayDataType type = DayDataType.Full)
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

            //(FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
