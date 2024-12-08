using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;

namespace AdventOfCode2024.Days
{
    class Day8 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            List<Node> empties = new List<Node>();
            Dictionary<char, List<Node>> antennaNodes = new Dictionary<char, List<Node>>();

            string[] data = DataLoader.LoadRowData(8, fullOrSimpleData);
            char[,] grid = new char[data[0].Length, data.Length];
            int startCursorPosFromTop = Console.CursorTop;
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    char c = data[y][x];
                    grid[x, y] = c;

                    if (c == '.')
                    {
                        Node newNode = new Node(new Int2(x, y), c);
                        //if (empties)
                        empties.Add(newNode);
                        TextUtilities.CFW("@DGy" + c);
                    }
                    else
                    {
                        Node newNode = new Node(new Int2(x, y), c);
                        if (antennaNodes.TryGetValue(c, out var nodes))
                        {
                            nodes?.Add(newNode);
                        }
                        else antennaNodes.Add(c, [newNode]);
                        TextUtilities.CFW("@Whi" + c);
                    }
                }
                Console.WriteLine();
            }

            DataMap<char> charMap = new DataMap<char>(grid);
            HashSet<Int2> uniqueAntinodes = new HashSet<Int2>();
            List<List<Int2>> allAntiNodeSets = new List<List<Int2>>();

            // For each set of nodes with the same character...
            foreach (var nodeSet in antennaNodes)
            {
                // ...we loop through all the nodes...
                foreach (var thisNode in nodeSet.Value)
                {
                    List<Int2> antinodes = new List<Int2>();
                    // ...to compare with eachother (except itself).
                    foreach (var otherNode in nodeSet.Value)
                    {
                        // Skip self
                        if (thisNode == otherNode) continue;

                        // Calculate the position of the antinode
                        Int2 diff = otherNode.Position - thisNode.Position;
                        Int2 newAntinodePos = thisNode.Position + 2 * diff;

                        // If outside map, don't count it
                        if (!charMap.IsInside(newAntinodePos)) continue;

                        // else we add them to the antinodes
                        antinodes.Add(newAntinodePos);
                        uniqueAntinodes.Add(newAntinodePos);
                        
                        // Write the antinode
                        Console.SetCursorPosition(newAntinodePos.x, startCursorPosFromTop + newAntinodePos.y);
                        TextUtilities.CFW("@Gre" + thisNode.Character);
                    }
                    allAntiNodeSets.Add(antinodes);
                }
            }

            // Move cursor down to below the map
            Console.SetCursorPosition(0, startCursorPosFromTop + charMap.Dim.y + 1);

            // Write result
            Console.WriteLine("Total antinodes:  " + allAntiNodeSets.Select(x=>x.Count).Sum());
            Console.WriteLine("Unique antinodes: " + uniqueAntinodes.Count);


            //Lookup<Node, Node> lookupList = new Lookup<Node, Node>();

        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }

        record Node(Int2 Position, char Character);
    }
}
