using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day18 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // I got 336 but it was wrong. It was apparently correct for someone else?
            // 294 was right :)   I just had to let the heuristic function be 0 to ensure we only go by the shortest path.

            // Set dim and size of datamap depending on full or simple dataset
            int dim = fullOrSimpleData == DayDataType.Simple ? 6+1 : 70+1;
            DataMap<int> dataMap = new DataMap<int>(dim * Int2.One, 0);

            // Create list of bricks from read data
            // I named it BRICK because BYTE is reserved in csharp
            Int2[] bricks = DataLoader.LoadRowData(18, fullOrSimpleData)
                .Select(row => row.Split(',')) // number split
                .Select(part => new Int2(int.Parse(part[0]), int.Parse(part[1]))).ToArray();

            // Let bytes fall into the map
            int maxIterations = fullOrSimpleData == DayDataType.Simple ? 12 : 1024;
            for (int i = 0; i < bricks.Length && i < maxIterations; i++)
            {
                Int2 brick = bricks[i];
                dataMap.grid[brick.x, brick.y] = 1;
            }

            // Print map
            int cursorPosTopStart = Console.CursorTop;
            PrintMap(dataMap);
            int cursorPosTopEnd = Console.CursorTop;

            // Find shortest path
            Int2 goal = dim * Int2.One - Int2.One;
            Int2 start = Int2.Zero;

            PriorityQueue<SimpleNode, int> pQueue = new PriorityQueue<SimpleNode, int>();
            pQueue.Enqueue(new SimpleNode(start, 0, null), 0);
            HashSet<Int2> visited = new HashSet<Int2>();
            SimpleNode? endNodeOfShortestPath = null;
            while (pQueue.Count > 0)
            {
                if (!pQueue.TryDequeue(out SimpleNode? n, out int cost))
                    break;

                if (visited.Contains(n.Position)) continue;

                Int2 pos = n.Position;

                // What this position contains in the map
                int mapValue = dataMap.grid[pos.x, pos.y];

                // If we hit a wall, abandon
                if (mapValue == 1)
                    continue;

                visited.Add(pos);

                Console.SetCursorPosition(0 + pos.x, cursorPosTopStart + pos.y);
                TextUtilities.CFW("@DReX");
                if (fullOrSimpleData == DayDataType.Simple) Thread.Sleep(50);

                // If we hit the goal we have found the shortest path
                if (pos == goal)
                {
                    endNodeOfShortestPath = n;
                    break;
                }

                // Queue adjacents
                foreach (var adjacent in dataMap.GetAllAdjacentPositions(pos))
                {
                    int adjMapValue = dataMap.grid[adjacent.x, adjacent.y];

                    // If we hit a wall, abandon
                    if (adjMapValue == 1 || visited.Contains(adjacent))
                        continue;

                    // The new priority in the queue should be the number of steps.
                    int newPrio = n.stepsToReach + 1;

                    // To make the algorithm faster, you can add a value that represents an approximation of how close you are.
                    // But to ensure we get the shortest path, we leave this at zero.
                    // Look up A-star heuristic function if you want to know more.
                    newPrio += 0;

                    pQueue.Enqueue(new SimpleNode(adjacent, n.stepsToReach + 1, n), newPrio);
                }
            }

            SimpleNode? t = endNodeOfShortestPath;
            int numberOfNodes = -1;
            while (t != null)
            {
                numberOfNodes++;
                Console.SetCursorPosition(0 + t.Position.x, cursorPosTopStart + t.Position.y);
                TextUtilities.CFW("@GreO");
                t = t.Previous;
            }
            Console.SetCursorPosition(0, cursorPosTopEnd);
            TextUtilities.CFWLine("@Gra >>> Number of nodes in shortest path: @Yel" + numberOfNodes);
        }

        record SimpleNode(Int2 Position, int stepsToReach, SimpleNode? Previous);

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // Set dim and size of datamap depending on full or simple dataset
            int dim = fullOrSimpleData == DayDataType.Simple ? 6 + 1 : 70 + 1;
            DataMap<int> dataMap = new DataMap<int>(dim * Int2.One, 0);

            // Create list of bricks from read data
            // I named it BRICK because BYTE is reserved in csharp
            Int2[] bricks = DataLoader.LoadRowData(18, fullOrSimpleData)
                .Select(row => row.Split(',')) // number split
                .Select(part => new Int2(int.Parse(part[0]), int.Parse(part[1]))).ToArray();

            // Make queue so we can drop one at a time
            Queue<Int2> brickQueue = new Queue<Int2>(bricks);

            // Let bytes fall into the map
            int maxIterations = fullOrSimpleData == DayDataType.Simple ? 12 : 1024;
            for (int i = 0; i < bricks.Length && i < maxIterations; i++)
            {
                Int2 brick = bricks[i];
                dataMap.grid[brick.x, brick.y] = 1;
            }

            // Find path until we can't
            Int2 goal = dim * Int2.One - Int2.One;
            Int2 start = Int2.Zero;
            Int2 droppedByte = Int2.Zero; // this is read in the end to see what step we stopped at
            SimpleNode? t = null;
            do
            {
                droppedByte = DropNextByte();
                t = FindExit();
            } while (t != null);


            // Print map
            int cursorPosTopStart = Console.CursorTop;
            PrintMap(dataMap);
            int cursorPosTopEnd = Console.CursorTop;

            // Draw red '#' at the position that will block
            Console.SetCursorPosition(0 + droppedByte.x, cursorPosTopStart + droppedByte.y);
            TextUtilities.CFW("@DRe#");

            // Print output
            Console.SetCursorPosition(0, cursorPosTopEnd);
            TextUtilities.CFWLine("@Gra >>> Byte that will block the path: @Yel" + droppedByte);

            Int2 DropNextByte()
            {
                Int2 brick = brickQueue.Dequeue();
                dataMap.grid[brick.x, brick.y] = 1;
                return brick;
            }

            SimpleNode? FindExit()
            {
                PriorityQueue<SimpleNode, int> pQueue = new PriorityQueue<SimpleNode, int>();
                pQueue.Enqueue(new SimpleNode(start, 0, null), 0);
                HashSet<Int2> visited = new HashSet<Int2>();
                SimpleNode? endNodeOfShortestPath = null;
                while (pQueue.Count > 0)
                {
                    if (!pQueue.TryDequeue(out SimpleNode? n, out int cost))
                        break;

                    if (visited.Contains(n.Position)) continue;

                    Int2 pos = n.Position;

                    // What this position contains in the map
                    int mapValue = dataMap.grid[pos.x, pos.y];

                    // If we hit a wall, abandon
                    if (mapValue == 1)
                        continue;

                    visited.Add(pos);

                    /*Console.SetCursorPosition(0 + pos.x, cursorPosTopStart + pos.y);
                    TextUtilities.CFW("@DReX");
                    if (fullOrSimpleData == DayDataType.Simple) Thread.Sleep(50);*/

                    // If we hit the goal we have found the shortest path
                    if (pos == goal)
                    {
                        endNodeOfShortestPath = n;
                        break;
                    }

                    // Queue adjacents
                    foreach (var adjacent in dataMap.GetAllAdjacentPositions(pos))
                    {
                        int adjMapValue = dataMap.grid[adjacent.x, adjacent.y];

                        // If we hit a wall, abandon
                        if (adjMapValue == 1 || visited.Contains(adjacent))
                            continue;

                        // The new priority in the queue should be the number of steps.
                        int newPrio = n.stepsToReach + 1;

                        // To make the algorithm faster, you can add a value that represents an approximation of how close you are.
                        // To ensure we get the shortest path we can leave this at zero.
                        // Look up A-star heuristic function if you want to know more.
                        // For part 2 I don't care about the shortest path. I just want the execution to be quicker.
                        newPrio += (int)(Int2.Distance(adjacent, goal) * 1000);

                        pQueue.Enqueue(new SimpleNode(adjacent, n.stepsToReach + 1, n), newPrio);
                    }
                }
                return endNodeOfShortestPath;
            }
        }

        void PrintMap(DataMap<int> map)
        {
            string s = "";
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    int val = map.grid[x, y];

                    if (val == 0) s += "@DGy.";
                    else if (val == 1) s += "@Gra#";
                }
                s += "\n";
            }
            TextUtilities.CFWLine(s);
        }
    }
}
