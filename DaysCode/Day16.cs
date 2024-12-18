using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;

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
            (Node startNode, Int2 goal, DataMap<char> map) parseOutput = Parse(DataLoader.LoadRowData(16, fullOrSimpleData));

            // Draw
            int cursorMapTop = Console.CursorTop;
            DrawMap(parseOutput.map);
            int cursorMapBottom = Console.CursorTop;

            PriorityQueue<Node, int> openQueue = new PriorityQueue<Node, int>();
            openQueue.Enqueue(parseOutput.startNode, parseOutput.startNode.Cost);

            HashSet<Int2> visited = new HashSet<Int2>();

            // Find optimal path
            Node? endPath = null;
            while (openQueue.Count > 0)
            {
                Node current = openQueue.Dequeue();
                visited.Add(current.Position);

                if (fullOrSimpleData == DayDataType.Simple)
                {
                    Console.SetCursorPosition(0 + current.Position.x, cursorMapTop + current.Position.y);
                    TextUtilities.CFW("@DGe.");
                    Thread.Sleep(1);
                }

                if (current.Position == parseOutput.goal)
                {
                    endPath = current;
                    continue;
                }

                // Queue next
                var adjacents = parseOutput.map.GetAllAdjacentPositions(current.Position);
                foreach (var adjacent in adjacents)
                {
                    if (parseOutput.map.grid[adjacent.x, adjacent.y] == '#') continue;

                    if (visited.Contains(adjacent)) continue;

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

            HashSet<Int2> allPlaces = new HashSet<Int2>();
            
            Node t = endPath!;

            //List<Node> alternateNodes = new List<Node>();
            //visitedDict.TryAdd((current.Position, current.Facing), current.Cost);
            Dictionary<(Int2 pos, Int2 dir), int> visitedDict = new Dictionary<(Int2 pos, Int2 dir), int>();
            List<Node> nodesInOrder = new List<Node>();

            // Draws the path
            while (t != null)
            {
                nodesInOrder.Add(t);
                visitedDict.Add((t.Position, t.Facing), t.Cost);
                
                // Dont draw if you hit a turn (only asthetic)
                if (t.Parent != null && t.Position == t.Parent.Position)
                {
                    t = t.Parent!;
                    continue;
                }

                Console.SetCursorPosition(t.Position.x, cursorMapTop + t.Position.y);
                char c = '%';
                if (t.Facing == Int2.Up) c = '^';
                else if (t.Facing == Int2.Right) c = '>';
                else if (t.Facing == Int2.Left) c = '<';
                else if (t.Facing == Int2.Down) c = 'v';

                TextUtilities.CFW("@DGe" + c);
                t = t.Parent!;

                Thread.Sleep(2);
            }

            // We try to walk from each crossroad and see if we can reunite with any part of the path
            List<Node> alternateNodes = new List<Node>();
            HashSet<Node> visitedNodes = new HashSet<Node>();
            HashSet<Int2> positionsFromBefore = new HashSet<Int2>(); // Keep a list of forbidden places
            for (int i = nodesInOrder.Count-1; i >= 0 ; i--)
            {
                Node currentStartNode = nodesInOrder[i];

                if (currentStartNode.Parent != null)
                    positionsFromBefore.Add(currentStartNode.Parent.Position);

                //Thread.Sleep(1000);
                //Console.SetCursorPosition(0, cursorMapTop);
                //DrawMap(parseOutput.map);

                openQueue.Clear();
                openQueue.Enqueue(currentStartNode, currentStartNode.Cost);

                // Step on the map
                do
                {
                    Node curNode = openQueue.Dequeue();
                    if (positionsFromBefore.Contains(curNode.Position))
                        continue;
                    if (curNode.Cost > endPath!.Cost)
                        continue;


                    if (curNode.Parent != null &&
                        curNode.Parent.Parent != null &&
                        curNode.Cost == curNode.Parent.Parent.Cost + 2000) // have rotated at least twice in a row
                        continue;

                    // If the next node exists in the dictionary, it means we've hit
                    // the shortest path
                    if (curNode != currentStartNode && visitedDict.TryGetValue((curNode.Position, curNode.Facing), out int existingCost))
                    {
                        if (curNode.Parent == currentStartNode)
                            continue;

                        if (curNode.Cost == existingCost)
                        {
                            alternateNodes.Add(curNode);
                            continue;
                        }

                        else if (curNode.Cost > existingCost)
                            continue;
                    }

                    // Draw progress
                    if (fullOrSimpleData == DayDataType.Simple)
                    {
                        Console.SetCursorPosition(0 + curNode.Position.x, cursorMapTop + curNode.Position.y);
                        if (curNode == currentStartNode)
                            TextUtilities.CFW("@Mgn+");
                        else TextUtilities.CFW("@DRe+");
                    }

                    var adjacents = parseOutput.map.GetAllAdjacentPositions(curNode.Position);
                    foreach (var adjacent in adjacents)
                    {
                        if (parseOutput.map.grid[adjacent.x, adjacent.y] == '#')
                            continue;

                        if (positionsFromBefore.Contains(adjacent))
                            continue; // don't add anything else if we've already been here

                        if (adjacent == curNode.Position + curNode.Facing) // Open space forward
                        {
                            //if (visited.Contains(curNode.Position + curNode.Facing))
                                //continue;

                            Node n = new Node(adjacent, curNode.Facing, curNode.Cost + 1, curNode);

                            //if (currentStartNode == n.Parent && nodesInOrder.Contains(n)) 
                            //    continue;

                            openQueue.Enqueue(n, n.Cost);

                            continue;
                        }

                        Int2 cw  = curNode.Facing.GetRotated90Clockwise(1);
                        Int2 ccw = curNode.Facing.GetRotated90Clockwise(3);

                        if (adjacent == curNode.Position + cw) // Rotate right
                        {
                            Node n = new Node(curNode.Position, cw, curNode.Cost + 1000, curNode);
                            //if (currentStartNode == n.Parent && nodesInOrder.Contains(n))
                            //    continue;

                            openQueue.Enqueue(n, n.Cost);
                        }

                        if (adjacent == curNode.Position + ccw) // Rotate left
                        {
                            Node n = new Node(curNode.Position, ccw, curNode.Cost + 1000, curNode);
                            //if (currentStartNode == n.Parent && nodesInOrder.Contains(n))
                            //    continue;

                            openQueue.Enqueue(n, n.Cost);
                        }
                    }


                } while (openQueue.Count > 0);
            }

            for (int i = 0; i < alternateNodes.Count; i++)
            {
                t = alternateNodes[i];

                while (t != null)
                {
                    //nodesInOrder.Add(t);
                    //visitedDict.Add((t.Position, t.Facing), t.Cost);

                    Console.SetCursorPosition(t.Position.x, cursorMapTop + t.Position.y);
                    char c = '%';
                    if (t.Facing == Int2.Up) c = '^';
                    else if (t.Facing == Int2.Right) c = '>';
                    else if (t.Facing == Int2.Left) c = '<';
                    else if (t.Facing == Int2.Down) c = 'v';

                    TextUtilities.CFW("@Mgn" + c);

                    if (t.Parent != null && t.Position == t.Parent.Position) // in a turn
                        t = t.Parent.Parent!;
                    else
                        t = t.Parent!;

                    Thread.Sleep(500);
                }
            }
            Thread.Sleep(5000);



            Console.SetCursorPosition(parseOutput.startNode!.Position.x, cursorMapTop + parseOutput.startNode.Position.y);
            TextUtilities.CFW("@RedS");
            Console.SetCursorPosition(parseOutput.goal.x, cursorMapTop + parseOutput.goal.y);
            TextUtilities.CFW("@RedE");
            Console.SetCursorPosition(0, cursorMapBottom);
            //TextUtilities.CFWLine($"@Gra >>> Lowest cost to node: @Yel{endNode!.Cost}");
            TextUtilities.CFWLine($"@Gra >>> Spots to visit: @Yel{allPlaces.Count}");
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
                    else if (c == '.')
                        grid[x, y] = '.';
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
                }
            }

            DataMap<char> map = new DataMap<char>(grid);

            return (startNode, goal, map);
        }
    }
}
