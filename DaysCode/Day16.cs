using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace AdventOfCode2024.Days
{
    class Day16 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // 91464 correct

            (Node startNode, Int2 goal, DataMap<char> map) parseOutput = Parse(DataLoader.LoadRowData(16, fullOrSimpleData));

            int cursorMapTop = Console.CursorTop;
            DrawMap(parseOutput.map);
            int cursorMapBottom = Console.CursorTop;

            PriorityQueue<Node, int> openQueue = new PriorityQueue<Node, int>();
            openQueue.Enqueue(parseOutput.startNode, parseOutput.startNode.Cost);

            HashSet<Int2> visited = new HashSet<Int2>();

            Node? endNode = null;
            while (openQueue.Count > 0)
            {
                Node current = openQueue.Dequeue();
                visited.Add(current.Position);

                if (fullOrSimpleData == DayDataType.Simple)
                {
                    Console.SetCursorPosition(0 + current.Position.x, cursorMapTop + current.Position.y);
                    TextUtilities.CFW("@GreX");
                    Thread.Sleep(50);
                }

                if (current.Position == parseOutput.goal)
                {
                    endNode = current;
                    break;
                }

                // Queue next
                var adjacents = parseOutput.map.GetAllAdjacentPositions(current.Position);
                foreach (var adjacent in adjacents)
                {
                    if (visited.Contains(adjacent)) continue;
                    if (parseOutput.map.grid[adjacent.x, adjacent.y] == '#') continue;

                    if (adjacent == current.Position + current.Facing) // Open space forward
                    {
                        Node n = new Node(adjacent, current.Facing, current.Cost + 1, current);
                        openQueue.Enqueue(n, n.Cost);
                        continue;
                    }

                    Int2 cw = current.Facing.GetRotated90Clockwise(1);
                    Int2 ccw = current.Facing.GetRotated90Clockwise(3);

                    if (adjacent == current.Position + cw) // Rotate
                    {
                        Node n = new Node(current.Position, cw, current.Cost + 1000, current);
                        openQueue.Enqueue(n, n.Cost);
                    }
                    if (adjacent == current.Position + ccw) // Rotate
                    {
                        Node n = new Node(current.Position, ccw, current.Cost + 1000, current);
                        openQueue.Enqueue(n, n.Cost);
                    }
                }
            }

            Node t = endNode!;
            while (t != null)
            {
                Console.SetCursorPosition(t.Position.x, cursorMapTop + t.Position.y);
                char c = '%';
                if (t.Facing == Int2.Up) c = '^';
                else if (t.Facing == Int2.Right) c = '>';
                else if (t.Facing == Int2.Left) c = '<';
                else if (t.Facing == Int2.Down) c = 'v';

                TextUtilities.CFW("@DGe" + c);

                if (t.Parent != null && t.Position == t.Parent.Position) // in a turn
                    t = t.Parent.Parent!;
                else
                    t = t.Parent!;
                Thread.Sleep(1);
            }

            Console.SetCursorPosition(parseOutput.startNode!.Position.x, cursorMapTop + parseOutput.startNode.Position.y);
            TextUtilities.CFW("@RedS");
            Console.SetCursorPosition(endNode!.Position.x, cursorMapTop + endNode.Position.y);
            TextUtilities.CFW("@RedE");
            Console.SetCursorPosition(0, cursorMapBottom);
            TextUtilities.CFWLine($"@Gra >>> Lowest cost to node: @Yel{endNode!.Cost}");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // Parse input
            (Node startNode, Int2 goal, DataMap<char> charMap) = Parse(DataLoader.LoadRowData(16, fullOrSimpleData));

            // Draw map
            int cursorTopStart = Console.CursorTop;
            DrawMap(charMap);
            int cursorTopEnd = Console.CursorTop;

            // Queue lists
            Queue<Node> queue = new Queue<Node>();
            List<Node> allShortestPathsToGoal = new List<Node>();
            
            DataMap<int> scoreMap = new DataMap<int>(charMap.Dim, 0);
            scoreMap.grid[startNode.Position.x, startNode.Position.y] = 0;


            //HashSet<Int2> visited = new HashSet<Int2>();
            //Dictionary<Int2, int> visitedScore = new Dictionary<Int2, int>();
            int lowestScore = int.MaxValue;

            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                Node node = queue.Dequeue();

                // Thanks for this idea, Ryan
                Walk(node.Facing, node.Cost + 1);
                Walk(node.Facing.GetRotated90Clockwise(1), node.Cost + 1001);
                Walk(node.Facing.GetRotated90Clockwise(3), node.Cost + 1001);

                void Walk(Int2 direction, int cost)
                {
                    Int2 pos = node.Position + direction;
                    char c = charMap.grid[pos.x, pos.y];

                    // Always do nothing if we hit a wall
                    if (c == '#')
                        return;


                    // If we find the goal we add the path to the list of paths
                    // If we find a path that is shorter than the other ones, we
                    // clear the list.
                    if (pos == goal)
                    {
                        if (cost <= lowestScore)
                        {
                            // If we found a path to the goal that was shorter
                            if (cost < lowestScore)
                            {
                                allShortestPathsToGoal.Clear();
                                lowestScore = cost;
                            }
                            allShortestPathsToGoal.Add(node);
                        }
                        return; // Don't add anything to the queue if we found the end
                    }

                    // The next step is to see if the position has already been visited.
                    // For some reason it has to be the position + direction (next position in front)
                    // "Int2 position = pos;" became
                    // "Int2 positionInFront = pos + direction;"
                    // I changed that because I discovered almost no places share the same cost/score.

                    Int2 positionInFront = pos + direction;
                    int scoreMapValueInFront = scoreMap.grid[positionInFront.x, positionInFront.y];

                    // If is unvisited, we give it a cost
                    if (scoreMapValueInFront == 0 || cost < scoreMapValueInFront)
                        scoreMap.grid[pos.x, pos.y] = cost;

                    // If we have a higher cost we have a shorter path there
                    else if (cost > scoreMapValueInFront)
                        return;

                    scoreMap.grid[pos.x, pos.y] = cost;
                    queue.Enqueue(new Node(pos, direction, cost, node));
                }
            }

            // Draw all shortest paths and count unique places
            HashSet<Int2> uniquePositions = new HashSet<Int2>() { goal };
            foreach (var shortestPath in allShortestPathsToGoal)
            {
                Node t = shortestPath;
                string color = TextUtilities.RandomColor();
                while (t != null)
                {
                    Console.SetCursorPosition(t.Position.x, cursorTopStart + t.Position.y);
                    TextUtilities.CFW(color + 'X');
                    uniquePositions.Add(t.Position);
                    t = t.Parent!;
                }
            }
            Console.SetCursorPosition(0, cursorTopEnd);
            TextUtilities.CFWLine("@Gra >>> Total unique places: @Yel" + uniquePositions.Count);
        }

        record Node(Int2 Position, Int2 Facing, int Cost, Node? Parent);

        void DrawMap(DataMap<char> map)
        {
            string s = "";
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    char c = map.grid[x, y];
                    if (c == '#')
                        s += "@Gra#";
                    else
                        s += "@Bla.";
                }
                s += '\n';
            }
            TextUtilities.CFWLine(s);
        }

        private (Node startNode, Int2 goal, DataMap<char> map) Parse(string[] data)
        {
            Node startNode = null!;
            Int2 goal = Int2.Zero;
            char[,] grid = new char[data[0].Length, data.Length];

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    char c = data[y][x];
                    if (c == '#')
                        grid[x, y] = '#';
                    else if (c == 'E')
                    {
                        goal = new Int2(x, y);
                        grid[x, y] = '.';
                    }
                    else if (c == 'S')
                    {
                        startNode = new Node(new Int2(x, y), Int2.Right, 0, null);
                        grid[x, y] = '.';
                    }
                    else
                        grid[x, y] = c;
                }
            }

            DataMap<char> map = new DataMap<char>(grid);

            return (startNode, goal, map);
        }
    }
}
