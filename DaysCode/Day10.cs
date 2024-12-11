using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.Drawing;
using static AdventOfCode2024.Days.Day10;

namespace AdventOfCode2024.Days
{
    class Day10 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // 778 correct answer.

            TopographicMap topographicMap = ParseMap(DataLoader.LoadRowData(10, fullOrSimpleData));
            /*for (int y = 0; y < topographicMap.Map.Dim.y; y++)
            {
                for (int x = 0; x < topographicMap.Map.Dim.x; x++)
                {
                    int height = topographicMap.Map.grid[x, y];

                    if ()
                }
            }*/

            // Print map
            int cursorTopPositionMapStart = Console.CursorTop;
            PrintMap(topographicMap);
            int cursorTopPositionMapEnd = Console.CursorTop;

            // Explore all trails
            topographicMap.ExploreAllTrails();

            // Write all trails
            foreach (var trail in topographicMap.Trailheads)
            {
                //Console.SetCursorPosition(0, cursorTopPositionMapStart);
                
                foreach (var pos in trail.positions)
                {
                    int height = topographicMap.Map.grid[pos.x, pos.y];
                    Console.SetCursorPosition(pos.x, pos.y + cursorTopPositionMapStart);
                    if (height == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        TextUtilities.CFW("@Bla" + height.ToString());
                        Console.ResetColor();
                    }
                    else if (height == 9)
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        TextUtilities.CFW("@Bla" + height.ToString());
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        TextUtilities.CFW("@Bla" + height.ToString());
                        Console.ResetColor();
                    }
                    //if (fullOrSimpleData == DayDataType.Simple) Thread.Sleep(50);
                }
                if (fullOrSimpleData == DayDataType.Simple)
                {
                    //Thread.Sleep(500);
                    Console.SetCursorPosition(0, cursorTopPositionMapStart);
                    PrintMap(topographicMap);
                }
            }
            Console.SetCursorPosition(0, cursorTopPositionMapEnd);

            TextUtilities.CFWLine("@Gra >>> Total score: @Yel" + topographicMap.GetScores());
            TextUtilities.CFWLine("@Gra >>> Total score: @Yel" + topographicMap.Trailheads.Count);

            /*foreach (var item in topographicMap.Trailheads)
            {
                Console.WriteLine($"Start: {topographicMap.Map.grid[item.positions[0].x, item.positions[0].y]}".PadRight(30) + (topographicMap.Map.grid[item.positions[^1].x, item.positions[^1].y]));
            }*/
        }

        private void PrintMap(TopographicMap topographicMap)
        {
            for (int y = 0; y < topographicMap.Map.Dim.y; y++)
            {
                for (int x = 0; x < topographicMap.Map.Dim.x; x++)
                {
                    int height = topographicMap.Map.grid[x, y];
                    string color = "@Gra";
                    if (height <= 0) color = "@DGy";
                    else if (height <= 1) color = "@DRe";

                    else if (height <= 3) color = "@Red";


                    else if (height <= 6) color = "@Yel";

                    else if (height <= 7) color = "@Cya";
                    else if (height <= 8) color = "@Gre";
                    if (height == 9) color = "@Mgn";
                    TextUtilities.CFW(color + height.ToString());
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            RunFirstStar(fullOrSimpleData);
        }

        public record Trailhead(List<Int2> positions);

        public TopographicMap ParseMap(string[] rowsOfData)
        {
            int[,] grid = new int[rowsOfData[0].Length, rowsOfData.Length];
            List<Trailhead> trailList = new List<Trailhead>();
            for (int y = 0; y < rowsOfData.Length; y++)
            {
                for (int x = 0; x < rowsOfData[0].Length; x++)
                {
                    grid[x,y] = (int)char.GetNumericValue(rowsOfData[y][x]);
                    if (grid[x, y] == 0) trailList.Add(new Trailhead([new Int2(x,y)]));
                }
            }
            return new TopographicMap(new DataMap<int>(grid), trailList);
        }
    
        public class TopographicMap
        {
            private DataMap<int> map = new DataMap<int>();
            private List<Trailhead> trailheads = new List<Trailhead>();

            public TopographicMap(DataMap<int> map, List<Trailhead> trailheads)
            {
                this.map = map;
                this.trailheads = trailheads;
            }

            public int GetScores()
            {
                int scores = 0;
                Dictionary<Int2, HashSet<Int2>> uniqueFirstPositions = new Dictionary<Int2, HashSet<Int2>>();

                foreach (var trail in trailheads)
                {
                    Int2 firstPos = trail.positions[0];
                    Int2 lastPos = trail.positions[^1];

                    if (!uniqueFirstPositions.TryAdd(firstPos, [lastPos]))
                    {
                        // If we failed to add the trail
                        // because hashset contains no duplicates we can do this to ensure we have unique last positions
                        uniqueFirstPositions[firstPos].Add(lastPos);
                    }
                }

                foreach (var kvp in uniqueFirstPositions)
                {
                    scores += kvp.Value.Count; // the length of unique nr of last positions
                }

                return scores;
            }

            public void ExploreAllTrails()
            {
                List<Trailhead> completedTrails = new List<Trailhead>();

                Stack<Trailhead> trailStack = new Stack<Trailhead>(trailheads);

                while (trailStack.Count > 0)
                {
                    Trailhead trail = trailStack.Pop();
                    int trailLastHeight = map.grid[trail.positions[^1].x, trail.positions[^1].y];
                    
                    if (trailLastHeight == 9) // If we've found a goal, add it to the completed paths
                    {
                        completedTrails.Add(trail);
                        continue;
                    }

                    // All adjacent are guaranteed to be within the bounds of the map
                    List<Int2> adjacents = map.GetAllAdjacentPositions(trail.positions[^1]);

                    for (int i = 0; i < adjacents.Count; i++)
                    {
                        Int2 p = adjacents[i];
                        int heightOfAdjacent = map.grid[p.x, p.y];
                        if (heightOfAdjacent == trailLastHeight + 1)
                        {
                            // Create new path
                            Trailhead newTrail = new Trailhead(new List<Int2>(trail.positions));
                            newTrail.positions.Add(p);
                            trailStack.Push(newTrail);
                        }
                    }
                }

                // When we've explored all, update the trail list
                trailheads = completedTrails;
            }

            public DataMap<int> Map => map;
            public List<Trailhead> Trailheads => trailheads;
        }
    }
}
