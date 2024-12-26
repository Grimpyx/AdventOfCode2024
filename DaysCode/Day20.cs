using AdventOfCode2024.Utilities;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day20 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            DataMap<char> map = DataMap<char>.GetCharMapFromRowData(DataLoader.LoadRowData(20, fullOrSimpleData));

            // Select start and end positions
            Int2 start = Int2.Zero;
            Int2 end = Int2.Zero;
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    char c = map.grid[x, y];
                    if (c == 'S') start = new Int2(x, y);
                    if (c == 'E') end = new Int2(x, y);
                }
            }

            Node defaultRoute = FindShortestPath(start, end, map).OrderBy(route => route.StepsTaken).First();

            Dictionary<int, int> dict = new Dictionary<int, int>();

            for (int i = 0; i < defaultRoute.StepsTaken; i++)
            {
                List<Node> routes = FindShortestPath(start, end, map, cheatAtStep: i, false)
                    .Where(route => route.StepsTaken < defaultRoute.StepsTaken)
                    .OrderBy(route => route.StepsTaken)
                    .ToList();

                foreach (var route in routes)
                {
                    int diff = defaultRoute.StepsTaken - route.StepsTaken;
                    
                    if (dict.ContainsKey(diff)) dict[diff]++;
                    else dict.Add(diff, 1);
                }
            }

            Console.WriteLine();
            foreach (var item in dict.OrderBy(i => i.Key))
            {
                TextUtilities.CFWLine($"@GraThere are @Yel{item.Value}@Gra cheats that save @Yel{item.Key}@Gra picoseconds.");
            }

            if (fullOrSimpleData == DayDataType.Full)
            {
                int sumofOver100 = dict.Where(e => e.Key >= 100).Select(e => e.Value).Sum();
                TextUtilities.CFWLine($"\n@Gra >>> Total amount of cheats that would save at least 100 picoseconds: @Yel{sumofOver100}");
            }
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // 2054116 too high!
            // 985482 correct!

            // My method of looping through x and y using NEGATIVE values didn't calculate the steps correctly.
            // I had to use Math.Abs(x) and Math.Abs(y).

            // This solution should work for part 1 as well, and it is sooo much better!
            // In part one this was the process:
            // Walk the map, perform cheat at designated step, and then keep walking the map to generate a path
            // Because there's no branching in the path, we walk it ONCE to assign a step value to each position.
            // Then, we can loop through the path, and then loop through all possible cheats from that position.
            // For each cheat we get a start and end position, and because we have already calculated how far it is
            // to each and every position we can easily calculate the difference (how many picoseconds gained). Superfast.

            DataMap<char> map = DataMap<char>.GetCharMapFromRowData(DataLoader.LoadRowData(20, fullOrSimpleData));

            // Select start and end positions
            Int2 start = Int2.Zero;
            Int2 end = Int2.Zero;
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    char c = map.grid[x, y];
                    if (c == 'S') start = new Int2(x, y);
                    if (c == 'E') end = new Int2(x, y);
                }
            }

            Node defaultRoute = FindShortestPath(start, end, map).OrderBy(route => route.StepsTaken).First();
            List<Int2> moveOrder = new List<Int2>(); // The list of all positions in order
            Dictionary<Int2, int> stepLookup = new Dictionary<Int2, int>();

            // Here we "walk the path" to set up our data: our dictionary and our moves in order
            Node tempNode = defaultRoute;
            while (tempNode != null)
            {
                moveOrder.Insert(0, tempNode.Position);
                stepLookup.Add(tempNode.Position, tempNode.StepsTaken);

                tempNode = tempNode.previous!;
            }

            Dictionary<int, int> diffLookup = new Dictionary<int, int>();
            for (int cheatStep = 0; cheatStep < moveOrder.Count; cheatStep++)
            {
                Int2 cheatStart = moveOrder[cheatStep];
                int stepsToReachCheatStart = stepLookup[cheatStart];

                // Loop through a square around our cheat start point
                for (int y = -20; y < 21; y++)
                {
                    for (int x = -20; x < 21; x++)
                    {
                        // Because we cannot walk diagonally, we have to add this check to ensure we haven't walked more than 20 picoseconds
                        int stepsTakenDuringCheat = Math.Abs(x) + Math.Abs(y);
                        if (stepsTakenDuringCheat > 20) continue; // if the step vector is longer than 20 steps

                        Int2 nextPos = cheatStart + new Int2(x, y);
                        if (!map.IsInside(nextPos)) continue; // if outside of map, continue
                        if (!stepLookup.ContainsKey(nextPos)) continue; // if we didnt land on the path, continue

                        // Steps to reach the cheat end must take into consideration how many steps taken during the cheat duration
                        int stepsToReachCheatEnd = stepLookup[nextPos] - stepsTakenDuringCheat;
                        int delta = stepsToReachCheatEnd - stepsToReachCheatStart;

                        if (delta > 0)
                        {
                            // Count all results
                            if (diffLookup.ContainsKey(delta)) diffLookup[delta]++;
                            else diffLookup.Add(delta, 1);
                        }
                    }
                }
            }

            // Print each cheat (similar to how it is presented on the AoC website
            foreach (var item in diffLookup.OrderBy(i => i.Key))
            {
                if ((fullOrSimpleData == DayDataType.Full && item.Key >= 100)
                    || (fullOrSimpleData == DayDataType.Simple && item.Key >= 50))
                    TextUtilities.CFWLine($"@GraThere are @Yel{item.Value}@Gra cheats that save @Yel{item.Key}@Gra picoseconds.");
            }

            // Output
            if (fullOrSimpleData == DayDataType.Full)
            {
                int sumofOver100 = diffLookup.Where(e => e.Key >= 100).Select(e => e.Value).Sum();
                TextUtilities.CFWLine($"\n@Gra >>> Total amount of cheats that would save at least @Yel100 @Grapicoseconds: @Yel{sumofOver100}");
            }
            else
            {
                int sumofOver50 = diffLookup.Where(e => e.Key >= 50).Select(e => e.Value).Sum();
                TextUtilities.CFWLine($"\n@Gra >>> Total amount of cheats that would save at least @Yel50 @Grapicoseconds: @Yel{sumofOver50}");
            }
        }

        record Node(Int2 Position, int StepsTaken, Node? previous);
        record Cheat(Int2 Start, Int2 End);

        List<Node> FindShortestPath(Int2 start, Int2 goal, DataMap<char> map, int cheatAtStep = -1, bool print = true)
        {
            // Queue
            Queue<Node> pQueue = new Queue<Node>();
            pQueue.Enqueue(new Node(start, 0, null)); // First element

            // Visited
            //HashSet<Int2> visited = new HashSet<Int2>();

            // Pr int
            int startCursor = Console.CursorTop;
            if (print) PrintMap(map);
            int endCursor = Console.CursorTop;

            //Node? endNode = null;
            List<Node> endNodes = new List<Node>();
            while (pQueue.Count > 0)
            {
                Node n = pQueue.Dequeue();

                //if (visited.Contains(n.Position))
                //    continue;
                //visited.Add(n.Position);

                if (n.Position == goal)
                {
                    endNodes.Add(n);
                    //endNode = n;
                    //break;
                    continue;
                }

                if (cheatAtStep < 0 || n.StepsTaken != cheatAtStep)
                {
                    foreach (var adjacentPos in map.GetAllAdjacentPositions(n.Position))
                    {

                        if (n.previous != null)
                        {
                            if (n.previous.Position == adjacentPos) continue;
                        }

                        char c = map.grid[adjacentPos.x, adjacentPos.y];

                        if (c == '#') continue;

                        Node nextNode = new Node(adjacentPos, n.StepsTaken + 1, n);
                        //int priority = nextNode.StepsTaken + (int)Int2.SqrDistance(adjacentPos, goal);
                        pQueue.Enqueue(nextNode);
                    }
                }
                else // Cheat
                {
                    // Add the closest adjacents
                    /*List<Int2> adjacents1step = map.GetAllAdjacentPositions(n.Position);
                    foreach (var adjacent in adjacents1step)
                    {
                        visited.Add(adjacent);
                    }*/

                    // Grab all positions two steps away and queue them
                    Int2[] adjacents2steps =
                    [
                        n.Position + (2 * Int2.Up),
                        n.Position + (2 * Int2.Down),
                        n.Position + (2 * Int2.Right),
                        n.Position + (2 * Int2.Left)
                    ];

                    foreach (var adjacentPos in adjacents2steps)
                    {
                        if (!map.IsInside(adjacentPos)) continue; // don't go backward

                        if (n.previous != null)
                        {
                            if (n.previous.previous != null)
                            {
                                if (n.previous.previous.Position == adjacentPos) continue;
                            }
                        }

                        char c = map.grid[adjacentPos.x, adjacentPos.y];

                        if (c == '#') continue;

                        Node nextNode = new Node(adjacentPos, n.StepsTaken + 2, n);
                        int priority = nextNode.StepsTaken + (int)Int2.SqrDistance(adjacentPos, goal);
                        pQueue.Enqueue(nextNode);
                    }
                }
                
            }

            if (print && endNodes.Count > 0)
            {
                Node writeNode = endNodes[0]!.previous!;
                while (writeNode.Position != start)
                {
                    Console.SetCursorPosition(writeNode.Position.x, startCursor + writeNode.Position.y);
                    TextUtilities.CFW("@DGeX");

                    writeNode = writeNode.previous!;
                }
                Console.SetCursorPosition(0, endCursor);
            }

            return endNodes;
        }


        List<Node> FindShortestPathP2(Int2 start, Int2 goal, DataMap<char> map, int cheatAtStep = -1, bool print = true)
        {
            // Queue
            Queue<Node> pQueue = new Queue<Node>();
            pQueue.Enqueue(new Node(start, 0, null)); // First element

            // Pr int
            int startCursor = Console.CursorTop;
            if (print) PrintMap(map);
            int endCursor = Console.CursorTop;

            List<Node> endNodes = new List<Node>();
            while (pQueue.Count > 0)
            {
                Node n = pQueue.Dequeue();

                bool cheat = (cheatAtStep != -1) && (n.StepsTaken >= cheatAtStep) && (n.StepsTaken < cheatAtStep + 20);

                //if (cheat)
                //    Console.WriteLine();

                if (n.Position == goal)
                {
                    endNodes.Add(n);
                    continue;
                }

                foreach (var adjacentPos in map.GetAllAdjacentPositions(n.Position))
                {
                    if (n.previous != null)
                    {
                        if (n.previous.Position == adjacentPos) continue;
                    }

                    char c = map.grid[adjacentPos.x, adjacentPos.y];

                    
                    if (c == '#' && !cheat) continue;

                    Node nextNode = new Node(adjacentPos, n.StepsTaken + 1, n);
                    pQueue.Enqueue(nextNode);
                }
            }

            if (print && endNodes.Count > 0)
            {
                Node writeNode = endNodes[0]!.previous!;
                while (writeNode.Position != start)
                {
                    Console.SetCursorPosition(writeNode.Position.x, startCursor + writeNode.Position.y);
                    TextUtilities.CFW("@DGeX");

                    writeNode = writeNode.previous!;
                }
                Console.SetCursorPosition(0, endCursor);
            }

            return endNodes;
        }


        /*
        Node? FindShortestPath(Int2 start, Int2 goal, DataMap<char> map, int cheatAtStep = -1, bool print = true)
        {
            // Queue
            PriorityQueue<Node, int> pQueue = new PriorityQueue<Node, int>();
            pQueue.Enqueue(new Node(start, 0, null), 0); // First element

            // Visited
            HashSet<Int2> visited = new HashSet<Int2>();

            // Pr int
            int startCursor = Console.CursorTop;
            if (print) PrintMap(map);
            int endCursor = Console.CursorTop;



            Node? endNode = null;
            while (pQueue.Count > 0)
            {
                Node n = pQueue.Dequeue();
                if (visited.Contains(n.Position))
                    continue;
                visited.Add(n.Position);

                if (n.Position == goal)
                {
                    endNode = n;
                    break;
                }

                if (cheatAtStep < 0 || n.StepsTaken != cheatAtStep)
                {
                    foreach (var adjacentPos in map.GetAllAdjacentPositions(n.Position))
                    {
                        char c = map.grid[adjacentPos.x, adjacentPos.y];

                        if (c == '#') continue;

                        Node nextNode = new Node(adjacentPos, n.StepsTaken + 1, n);
                        int priority = nextNode.StepsTaken + (int)Int2.SqrDistance(adjacentPos, goal);
                        pQueue.Enqueue(nextNode, priority);
                    }
                }
                else // Cheat
                {
                    // Add the closest adjacents
                    List<Int2> adjacents1step = map.GetAllAdjacentPositions(n.Position);
                    foreach (var adjacent in adjacents1step)
                    {
                        visited.Add(adjacent);
                    }

                    // Grab all positions two steps away and queue them
                    Int2[] adjacents2steps =
                    [
                        n.Position + (2 * Int2.Up),
                        n.Position + (2 * Int2.Down),
                        n.Position + (2 * Int2.Right),
                        n.Position + (2 * Int2.Left)
                    ];

                    foreach (var adjacentPos in adjacents2steps)
                    {
                        if (!map.IsInside(adjacentPos)) continue;
                        char c = map.grid[adjacentPos.x, adjacentPos.y];

                        if (c == '#') continue;

                        Node nextNode = new Node(adjacentPos, n.StepsTaken + 2, n);
                        int priority = nextNode.StepsTaken + (int)Int2.SqrDistance(adjacentPos, goal);
                        pQueue.Enqueue(nextNode, priority);
                    }
                }
                
            }

            if (print)
            {
                Node writeNode = endNode!.previous!;
                while (writeNode.Position != start)
                {
                    Console.SetCursorPosition(writeNode.Position.x, startCursor + writeNode.Position.y);
                    TextUtilities.CFW("@DGeX");

                    writeNode = writeNode.previous!;
                }
                Console.SetCursorPosition(0, endCursor);
            }

            return endNode;
        }
*/

        void PrintMap(DataMap<char> map)
        {
            string s = "";
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    char c = map.grid[x, y];
                    string color;
                    if (c == '#') color = "@DGy";
                    else if (c == '.') color = "@Bla";
                    else if (c == 'E') color = "@Mgn";
                    else if (c == 'S') color = "@Mgn";
                    else color = "Gra";

                    s += color + c;
                }
                s += "\n";
            }
            TextUtilities.CFWLine(s);
        }
    }
}
