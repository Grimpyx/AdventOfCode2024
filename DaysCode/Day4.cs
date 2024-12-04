using AdventOfCode2024.Utilities;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AdventOfCode2024.Days
{
    class Day4 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // Load and interpret data
            string[] rowData = DataLoader.LoadRowData(4, fullOrSimpleData);
            Element[,] grid = new Element[rowData[0].Length, rowData.Length]; // grid[x,y], counting from the top left
            for (int x = 0; x < rowData[0].Length; x++) // x
            {
                for (int y = 0; y < rowData.Length; y++) // y
                {
                    grid[y, x] = (Element)rowData[y][x];
                }
            }

            DataMap<Element> map = new DataMap<Element>(grid);

            // Print data
            PrintGrid(map);

            // StartProcess
            int total = 0;
            PriorityQueue<Int2, int> pQueue = new PriorityQueue<Int2, int>();
            //Queue<(Int2 pos, int step)> queue = new Queue<(Int2 pos, int step)>();

            int cursorTopPosStart = Console.CursorTop;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    // Starting point. Only queue element and go on with the find-adjacent-process if we start at an X
                    if (map.grid[x, y] == Element.X)
                        pQueue.Enqueue(new Int2(x, y), ElementToInt(Element.X));
                    else continue;

                    // Dequeue and queue the next in line
                    Element nextLetter; List<Int2> adjacent; Int2 pos; int prioValue;
                    while (pQueue.TryDequeue(out pos, out prioValue))
                    {
                        // If letter is S we add to the counter and continue
                        if (prioValue == ElementToInt(Element.S))
                        {
                            total++;
                            continue;
                        }

                        // Add adjacent that match the next letter
                        nextLetter = IntToElement(prioValue + 1);
                        adjacent = GetAllAdjacent(map, pos, nextLetter);
                        pQueue.EnqueueRange(adjacent, prioValue + 1);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.SetCursorPosition(x, y + cursorTopPosStart);
                        Console.Write('X');
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        foreach (var item in pQueue.UnorderedItems)
                        {
                            Console.SetCursorPosition(item.Element.x, item.Element.y + cursorTopPosStart);
                            Console.Write(map.grid[item.Element.x, item.Element.y]);
                        }
                        Console.ResetColor();
                        Console.Read();
                    }
                }
            }

            Console.Write(" >>> Found a total of ");
            TextUtilities.ColorWrite(ConsoleColor.Yellow, total.ToString());
            Console.WriteLine(" 'XMAS'");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }

        enum Element
        {
            X = 'X',
            M = 'M',
            A = 'A',
            S = 'S'
        }

        private void PrintGrid(DataMap<Element> map)
        {
            Element[,] grid = map.grid;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Console.Write(grid[x, y]);
                }
                Console.WriteLine();
            }
        }

        private List<Int2> GetAllAdjacent(DataMap<Element> map, Int2 origin, Element targetElementType)
        {
            // Create list to be returned later
            List<Int2> adjacentPoints = new List<Int2>();

            // Look at all points around and add all that matches with the target element
            for (int i = 0; i < 8; i++)
            {
                // Each iteration grabs a new vector rotated 45 degrees
                Int2 v = origin + GetRotatedVector(i);

                if (IsInside(v) && map.grid[v.x,v.y] == targetElementType)
                    adjacentPoints.Add(v);
            }
            return adjacentPoints;

            Int2 GetRotatedVector(int variable) // 0-8, otherwise is %8
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

            bool IsInside(Int2 point) =>
                (point.x >= 0 && point.x < map.Dim.x) &&    // Within x bounds
                (point.y >= 0 && point.y < map.Dim.y);      // Within y bounds
        }


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

        struct DataMap<T>
        {
            public T[,] grid;

            public DataMap(T[,] grid)
            {
                this.grid = grid;
                Dim = new Int2(grid.GetLength(0), grid.GetLength(1));
            }

            public Int2 Dim { get; private set; }
        }
    }

   
}
