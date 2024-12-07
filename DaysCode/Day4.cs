using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2024.Days
{
    class Day4 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // Settings
            bool instantDraw = true; // takes too long if you dont draw instantly

            // Load and interpret data
            string[] rowData = DataLoader.LoadRowData(4, fullOrSimpleData);
            Element[,] grid = new Element[rowData[0].Length, rowData.Length]; // grid[x,y], counting from the top left
            for (int y = 0; y < rowData.Length; y++) // y
            {
                for (int x = 0; x < rowData[0].Length; x++) // x
                {
                    grid[x, y] = (Element)rowData[y][x];
                }
            }

            DataMap<Element> map = new DataMap<Element>(grid);

            // Print data
            Console.ForegroundColor = ConsoleColor.DarkGray;
            int cursorTopPosStart = Console.CursorTop;
            int cursorTopPosEnd = cursorTopPosStart + map.Dim.y;
            PrintGrid(map);

            // StartProcess
            int total = 0;
            PriorityQueue<Int2, int> pQueue = new PriorityQueue<Int2, int>();
            //Queue<(Int2 pos, int step)> queue = new Queue<(Int2 pos, int step)>();


            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Int2 start = new Int2(x, y);

                    if (map.grid[start.x, start.y] != Element.X) continue;

                    // Loop through all directions
                    for (int dirIndex = 0; dirIndex < 8; dirIndex++)
                    {
                        Int2 dirVector = GetRotatedVector(dirIndex);
                        //if (!IsInside(map, dirVector)) continue;

                        // Steps to walk in each direction
                        for (int i = 1; i < 4; i++)
                        {
                            Int2 nextVector = start + i * dirVector;

                            if (!IsInside(map, nextVector) || map.grid[nextVector.x, nextVector.y] != IntToElement(i)) break;

                            if (i == 3) // If found S
                            {
                                total++;

                                // The following draws the letters in the console window
                                for (int j = 0; j < 4; j++)
                                {
                                    Int2 v = start + j * dirVector;
                                    if (j==0) Console.ForegroundColor = ConsoleColor.Red;
                                    else Console.ForegroundColor = ConsoleColor.Yellow;

                                    Console.SetCursorPosition(v.x, v.y + cursorTopPosStart);
                                    Console.Write(IntToElement(j));
                                    Console.ResetColor();
                                    if (!instantDraw) Thread.Sleep(25);
                                }
                                // Update 'score' tracker
                                Console.SetCursorPosition(0, cursorTopPosEnd);
                                Console.Write("\nTotal 'XMAS' found: ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine(total);
                                Console.ResetColor();
                                if (!instantDraw) Thread.Sleep(100);
                            }
                        }
                    }

                }
            }

            // Print output
            Console.Write("\n >>> Found a total of ");
            TextUtilities.ColorWrite(ConsoleColor.Yellow, total.ToString());
            Console.WriteLine(" 'XMAS'");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // Patterns we're looking for:
            //   M . S  |  M . M  |  S . M  |  S . S
            //   . A .  |  . A .  |  . A .  |  . A .
            //   M . S  |  S . S  |  S . M  |  M . M
            // Common denominator, all patterns start with an A in the middle and disregard horizontal and vertical directions
            // If one diagonal is different, the other must be different as long as we have a maximum of 2 adjacent M and 2 adjacent S

            // Counter for how many we find
            int total = 0;

            // Load and interpret data
            string[] rowData = DataLoader.LoadRowData(4, fullOrSimpleData);
            Element[,] grid = new Element[rowData[0].Length, rowData.Length]; // grid[x,y], counting from the top left
            for (int y = 0; y < rowData.Length; y++) // y
            {
                for (int x = 0; x < rowData[0].Length; x++) // x
                {
                    grid[x, y] = (Element)rowData[y][x];
                }
            }
            DataMap<Element> map = new DataMap<Element>(grid);

            // Print data
            Console.ForegroundColor = ConsoleColor.DarkGray;
            int cursorTopPosStart = Console.CursorTop;
            int cursorTopPosEnd = cursorTopPosStart + map.Dim.y;
            PrintGrid(map);

            List<Element> adjacents = new List<Element>();

            // Loop through all coordinates. We start at row/col 1 and end one step before the limit o
            for (int y = 1; y < grid.GetLength(1) - 1; y++)
            {
                for (int x = 1; x < grid.GetLength(0) - 1; x++)
                {
                    if (map.grid[x,y] != Element.A) continue; // if it's not an 'A' we can't be in the middle of an X-MAS

                    adjacents.Clear();

                    Int2 center = new Int2(x, y); // X-MAS center

                    int m_counter = 0, s_counter = 0;
                    for (int i = 1; i < 8; i += 2)
                    {
                        // will be a diagonal away from the current center point
                        Int2 next = center + GetRotatedVector(i);
                        Element letter = map.grid[next.x, next.y];

                        // Count 'M':s and 'S':s
                        if (letter == Element.M) m_counter++;
                        else if (letter == Element.S) s_counter++;

                        // If we have more than two 'M':s or 'S':s, or if the diagonal contains an 'X' or 'A'
                        // it can't be an X-MAS
                        if (m_counter > 2 || s_counter > 2 || letter == Element.X || letter == Element.A)
                        {
                            // Reset the lest and counters and move on to the next coordinate
                            adjacents.Clear();
                            break;
                        }
                        adjacents.Add(letter);
                    }

                    // If we have four corners with only 'M' or 'S' (Max 2M and 2S)
                    // we can move on to the last check
                    if (adjacents.Count == 4)
                    {
                        // If one diagonal contains different elements it must contain one M and one S.
                        // It logically follows that the other diagonal must be the same meaning we have 'X-MAS'
                        if (adjacents[0] != adjacents[2])
                        {
                            // Increase the counter for how many 'X-MAS' we have
                            total++;

                            // Write middle coordinate ('A') of the 'X-MAS'
                            if (fullOrSimpleData == DayDataType.Simple) Console.ReadKey();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.SetCursorPosition(x, y + cursorTopPosStart);
                            Console.Write('A');

                            // Loop through the diagonal directions and write the out.
                            // 1 = up right, 3 = down right, 5 = down left, 7 = up left
                            for (int i = 1; i < 8; i += 2)
                            {
                                Int2 next = center + GetRotatedVector(i); // will be a diagonal away from the current center point

                                Element letter = map.grid[next.x, next.y];

                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.SetCursorPosition(next.x, next.y + cursorTopPosStart);
                                Console.Write(letter);
                                if (fullOrSimpleData == DayDataType.Simple) Thread.Sleep(50);
                            }
                            Console.ResetColor();

                            // Update 'score' tracker in console window
                            Console.SetCursorPosition(0, cursorTopPosEnd);
                            Console.Write("\nTotal 'XMAS' found: ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(total);
                            Console.ResetColor();
                            if (fullOrSimpleData == DayDataType.Simple) Thread.Sleep(300);
                        }
                    }
                }
            }

            // Print output
            Console.SetCursorPosition(0, cursorTopPosEnd);
            Console.Write("\n >>> Found a total of ");
            TextUtilities.ColorWrite(ConsoleColor.Yellow, total.ToString());
            Console.WriteLine(" 'XMAS'");
        }

        // Prints the grid.
        private void PrintGrid(DataMap<Element> map)
        {
            Element[,] grid = map.grid;

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    Console.Write(grid[x, y]);
                }
                Console.WriteLine();
            }
        }

        public List<Int2> GetAllAdjacent(DataMap<Element> map, Int2 origin, Element targetElementType)
        {
            // Create list to be returned later
            List<Int2> adjacentPoints = new List<Int2>();

            // Look at all points around and add all that matches with the target element
            for (int i = 0; i < 8; i++)
            {
                // Each iteration grabs a new vector rotated 45 degrees
                Int2 v = origin + GetRotatedVector(i);

                if (IsInside(map, v) && map.grid[v.x,v.y] == targetElementType)
                    adjacentPoints.Add(v);
            }
            return adjacentPoints;
        }

        public Int2 GetRotatedVector(int variable) // 0-8, otherwise is %8
        {
            // You can do this with a mathematic function, but to save computation I will hardcode below
            // Paste these lines into desmos to see
            // x_{1}\left(x\right)=\operatorname{round}\left(\sin\left(\frac{\pi}{4}x\right)\right)
            // y_{1}\left(x\right)=\operatorname{round}\left(\sin\left(\frac{\pi}{4}\left(2-x\right)\right)\right)
            // \left(x_{1}\left(n\right),y_{1}\left(n\right)\right)
            // n=0
            // ^ let n be constrained to 0 and 7 and you will see the coordinate point 'p' move around, starting at 12 o'clock

            // constrain to [0,7]
            // % does this: divides by, and grabs the Rest. 19 8*2+3, so 3 is our rest
            variable %= 8;

            switch (variable)
            {
                case 0: return new Int2(0, 1);      // Up
                case 1: return new Int2(1, 1);      // Up right
                case 2: return new Int2(1, 0);      // Right
                case 3: return new Int2(1, -1);     // Down right
                case 4: return new Int2(0, -1);     // Down
                case 5: return new Int2(-1, -1);    // Down left
                case 6: return new Int2(-1, 0);     // Left
                case 7: return new Int2(-1, 1);     // Up left
                default: return new Int2(0, 0);     // default case
            }
        }

        public bool IsInside<T>(DataMap<T> map, Int2 point) =>
            (point.x >= 0 && point.x < map.Dim.x) &&    // Within x bounds
            (point.y >= 0 && point.y < map.Dim.y);      // Within y bounds


        Element IntToElement(int integer)
        {
            return integer switch
            {
                0 => Element.X,
                1 => Element.M,
                2 => Element.A,
                _ => Element.S,
            };
        }
        int ElementToInt(Element element)
        {
            return element switch
            {
                Element.X => 0,
                Element.M => 1,
                Element.A => 2,
                _ => 3
            };
        }
    }
    enum Element
    {
        X = 'X',
        M = 'M',
        A = 'A',
        S = 'S'
    }


}
