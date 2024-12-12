using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace AdventOfCode2024.Days
{
    class Day12 : IDayChallenge
    {
        private bool isRunningPart2 = false;

        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            DataMap<char> plotMap = ParseMap(DataLoader.LoadRowData(12, fullOrSimpleData));
            HashSet<Int2> exploredPositions = new HashSet<Int2>();

            List<Plot> allPlots = new List<Plot>();


            // Print map once, such that we can draw on it
            int mapStartPosY = Console.CursorTop;
            for (int y = 0; y < plotMap.Dim.y; y++)
            {
                for (int x = 0; x < plotMap.Dim.x; x++)
                {
                    TextUtilities.CFW(plotMap.grid[x, y].ToString());
                }
                Console.WriteLine();
            }
            int mapEndPosY = Console.CursorTop;


            // Assign plots
            for (int y = 0; y < plotMap.Dim.y; y++)
            {
                for (int x = 0; x < plotMap.Dim.x; x++)
                {
                    Int2 loopPosition = new Int2(x, y);
                    if (exploredPositions.Contains(loopPosition))
                        continue; // continue if this is already part of a plot
                    exploredPositions.Add(loopPosition);

                    // Information about this plot position
                    char identifier = plotMap.grid[x, y];
                    Stack<Int2> plotsToAnalyze = new Stack<Int2>();

                    // Give each area a random color
                    string randomColor = TextUtilities.RandomColor();
                    Console.SetCursorPosition(loopPosition.x, loopPosition.y + mapStartPosY);
                    TextUtilities.CFW(randomColor + identifier);

                    // Create new plotobject
                    Plot newPlotObject = new Plot(identifier);
                    newPlotObject.Plots.Add(loopPosition);

                    // First iteration start from *here*
                    plotsToAnalyze.Push(loopPosition);

                    // Keep looking forward
                    while (plotsToAnalyze.Count > 0)
                    {
                        Int2 current = plotsToAnalyze.Pop();

                        var adjacents = plotMap.GetAllAdjacentPositions(current);
                        foreach (var adjacent in adjacents)
                        {
                            if (plotMap.grid[adjacent.x, adjacent.y] == identifier
                                && !exploredPositions.Contains(adjacent))
                            {
                                plotsToAnalyze.Push(adjacent);
                                newPlotObject.Plots.Add(adjacent);
                                exploredPositions.Add(adjacent);

                                Console.SetCursorPosition(adjacent.x, adjacent.y + mapStartPosY);
                                TextUtilities.CFW(randomColor + identifier);
                            }
                        }
                    }
                    allPlots.Add(newPlotObject);
                }
            }

            // Sets up output (see below)
            Console.SetCursorPosition(0, mapEndPosY + 1);
            TextUtilities.CFWLine("@Gra >>> Total plot groups: @Yel" + allPlots.Count);
            if (fullOrSimpleData == DayDataType.Simple)
            {
                TextUtilities.CFWLine("\n@YelArea     @Gra*   @YelPerimeter   @Gra=   @YelPrice");
                TextUtilities.CFWLine("@DGy" + new string('-', 50));
            }

            // Calculates the output, the area multiplied with the perimeter
            int totalPrice = 0;
            foreach (var plot in allPlots)
            {
                int area = plot.GetArea();
                int perimeter;
                if (!isRunningPart2)
                    perimeter = plot.GetPerimeterP1();
                else
                    perimeter = plot.GetPerimeterP2();

                totalPrice += area * perimeter;
                if (fullOrSimpleData == DayDataType.Simple) TextUtilities.CFWLine($"@Yel{area,-8} @Gra*   @Yel{perimeter,-7}     @Gra=   @Yel{area * perimeter}".PadRight(60) + " @GraTotal: @Yel" + totalPrice);
            }
            TextUtilities.CFWLine("@Gra >>> Sum of price: @Yel" + totalPrice);
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            isRunningPart2 = true;
            RunFirstStar(fullOrSimpleData);
            isRunningPart2 = false;
        }

        private DataMap<char> ParseMap(string[] data)
        {
            char[,] grid = new char[data[0].Length, data.Length];

            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    grid[x, y] = data[y][x];
                }
            }

            return new DataMap<char>(grid);
        }

        public class Plot
        {
            private HashSet<Int2> plots = new HashSet<Int2>();
            private char identifier;

            //private int area;
            //private int perimeter;

            public Plot(char identifier)
            {
                this.identifier = identifier;
            }

            public HashSet<Int2> Plots { get => plots; }
            public char Identifier { get => identifier; }
            /*public int Area
            {
                get
                {
                    if (area == 0)
                        area = GetArea();
                    return area;
                }
            }
            public int Perimeter
            {
                get
                {
                    if (perimeter == 0)
                        perimeter = GetPerimeterP1();
                    return perimeter;
                }
            }*/

            public int GetArea()
            {
                return plots.Count;
            }
            public int GetPerimeterP1()
            {
                int p = 0;
                foreach (Int2 from in plots)
                {
                    Int2 nextPos = from + Int2.Up;
                    if (!plots.Contains(nextPos)) p++;

                    nextPos = from + Int2.Down;
                    if (!plots.Contains(nextPos)) p++;

                    nextPos = from + Int2.Right;
                    if (!plots.Contains(nextPos)) p++;

                    nextPos = from + Int2.Left;
                    if (!plots.Contains(nextPos)) p++;
                }
                return p;
            }

            public int GetPerimeterP2()
            {
                int uniquePerimeterEdges = 0;

                // In this list, the first
                List<(Int2 position, Int2 normal)> listOfEdges = new List<(Int2, Int2)>();
                //HashSet<(Int2 position, Int2 normal)> listOfUnbrokenLines = new HashSet<(Int2, Int2)>();
                Int2[] directions = [Int2.Up, Int2.Down, Int2.Right, Int2.Left];

                // Find all edges and where they are pointing
                foreach (Int2 position in Plots)
                {
                    for (int dirId = 0; dirId < directions.Length; dirId++)
                    {
                        Int2 edir = directions[dirId];
                        Int2 epos = position + edir;

                        if (!plots.Contains(epos)) // it is at the boundary just outside
                        {
                            listOfEdges.Add((epos, edir));
                        }
                    }
                }

                HashSet<(Int2 position, Int2 normal)> exploredEdges = new HashSet<(Int2, Int2)>();
                foreach (var (thisPosition, thisNormal) in listOfEdges)
                {
                    // Skip explored
                    if (exploredEdges.Contains((thisPosition, thisNormal))) continue;
                    exploredEdges.Add((thisPosition, thisNormal));
                    uniquePerimeterEdges++;


                    // We try to find all edges that align in the same direction around and don't count them toward the total amount of edges
                    Stack<Int2> nextEdgeStack = new Stack<Int2>();
                    nextEdgeStack.Push(thisPosition);
                    while (nextEdgeStack.Count > 0)
                    {
                        Int2 p = nextEdgeStack.Pop();
                        for (int dirId = 0; dirId < directions.Length; dirId++)
                        {
                            Int2 nextPos = p + directions[dirId];
                            if (!listOfEdges.Contains((nextPos, thisNormal)) || exploredEdges.Contains((nextPos, thisNormal)))
                                continue; // If it doesn't exist continue

                            // If the edge did exist, it's part of this line.
                            // We add it to the stack to check if the edge continues,
                            // and also add it to the explored edges such that we know not
                            // to check it again.
                            nextEdgeStack.Push(nextPos);
                            exploredEdges.Add((nextPos, thisNormal));
                        }
                    }
                }

                return uniquePerimeterEdges;

                /*foreach (var (thisPosition, thisNormal) in listOfEdges)
                {
                    foreach (var (otherPosition, otherNormal) in listOfEdges)
                    {
                        if (thisNormal == otherNormal)
                        {
                            Int2 rotated90 = thisNormal.GetRotated90Clockwise();
                            Int2 rotated90Neg = -1 * rotated90;

                            Int2 diff = otherPosition - thisPosition;

                            // If part of the same line
                            if (diff == rotated90 || diff == rotated90Neg)

                        }
                    }
                }*/

                
            }
        }
    }
}
