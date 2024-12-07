using AdventOfCode2024.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Reflection.Metadata;

namespace AdventOfCode2024.Days
{
    class Day6 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // animation speed settings
            bool instantDraw = fullOrSimpleData == DayDataType.Simple ? false : true;

            string[] data = DataLoader.LoadRowData(6, fullOrSimpleData);
            DataMap<Tile> map = new DataMap<Tile>(new Tile[data[0].Length, data.Length]);
            Guard guard = new Guard(Int2.Zero, Int2.Up);

            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    Tile tile = (Tile)data[y][x];
                    if (data[y][x] == '^')
                    {
                        guard = new Guard(new Int2(x, y), Int2.Up);
                        map.grid[x, y] = Tile.Empty;
                    }
                    else map.grid[x, y] = tile;
                }
            }

            // Draw map
            int startHeightForMap = Console.CursorTop;
            for(int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    char c = (char)map.grid[x, y];
                    if (c == '.') Console.ForegroundColor = ConsoleColor.DarkGray;
                    else if (c == '#') Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(c);
                }
                Console.WriteLine();
            }

            // Walk the guard
            while (guard.Walk(map) == 1)
            {
                Int2 v = guard.Position;
                Console.SetCursorPosition(v.x, startHeightForMap + v.y);
                TextUtilities.CFW("@GreG");

                v -= guard.FacingDirection;
                Console.SetCursorPosition(v.x, startHeightForMap + v.y);
                TextUtilities.CFW("@DGrX");

                if (!instantDraw) Thread.Sleep(15);
            }
            Console.SetCursorPosition(0, startHeightForMap + map.Dim.y);
            Console.WriteLine();

            TextUtilities.CFWLine($"@Gra >>> Total path: @Yel{guard.Path.Count}");
            TextUtilities.CFWLine($"@Gra >>> Distinct:   @Yel{guard.DistinctPlaces.Count}");

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // Idea:
            // To find where we can place an obstruciton to create a loop we need to
            // 1. Somewhere on his initial path, otherwise we know he will not walk into it
            // 2. It needs to be placed such that the position the guard will rotate from will lead to him walking into a previous path.
            //
            // Essentially, It need to be positioned in front of a cross road.

            // 1644 too high
            // 1526 too high
            // 1483 too low
            // I got a 1482 but that should be too low

            // Problem discussion:
            // Read about a solution on reddit, but my method was the same.
            // I rewrote my method without making any changes to the idea and it just worked.
            // Before I placed a rock in front of the current path position. In my rewritten code I instead place
            // the rock on the path position, and create a guard one step behind.
            // 1516 was correct.

            string[] data = DataLoader.LoadRowData(6, fullOrSimpleData);
            DataMap<Tile> map = new DataMap<Tile>(new Tile[data[0].Length, data.Length]);
            Guard2 guard = new Guard2(Int2.Zero, Int2.Up);
            Int2 initialGuardStart = Int2.Zero;

            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    Tile tile = (Tile)data[y][x];
                    if (data[y][x] == '^')
                    {
                        initialGuardStart = new Int2(x, y);
                        guard = new Guard2(initialGuardStart, Int2.Up);
                        map.grid[x, y] = Tile.Empty;
                    }
                    else map.grid[x, y] = tile;
                }
            }

            // Draw map
            int startHeightForMap = Console.CursorTop;
            for (int y = 0; y < data.Length; y++)
            {
                string[] split = data[y].Split('#');
                for (int i = 0; i < split.Length; i++)
                {
                    if (i != split.Length-1)
                        TextUtilities.CFW($"@DGy{split[i]}@Gra#");
                    else
                        TextUtilities.CFW($"@DGy{split[i]}");
                }
                Console.WriteLine();
            }

            // Walk the guard for the initial complete path from start
            do { } while (guard.Walk(map) == 1);

            // Draw path
            foreach (var pathElement in guard.Path)
            {
                Int2 v = pathElement.Key;
                Console.SetCursorPosition(v.x, startHeightForMap + v.y);

                if (pathElement.Value.Count > 1)
                {
                    TextUtilities.CFW("@DRe+");
                }
                else
                {
                    if (pathElement.Value[0] == Int2.Right)
                        TextUtilities.CFW("@DYe>");
                    if (pathElement.Value[0] == Int2.Left)
                        TextUtilities.CFW("@DYe<");
                    if (pathElement.Value[0] == Int2.Up)
                        TextUtilities.CFW("@DYe^");
                    if (pathElement.Value[0] == Int2.Down)
                        TextUtilities.CFW("@DYev");
                }
            }
            Console.SetCursorPosition(guard.Position.x, startHeightForMap + guard.Position.y);
            TextUtilities.CFW("@MgnX");

            Console.SetCursorPosition(0, startHeightForMap + map.Dim.y);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();


            // Attempt to find all places you can place the rock
            // We do this by placing a rock and moving it along the path.
            HashSet<Int2> totalRockCandidates = new HashSet<Int2>();
            foreach (var item in guard.Path)
            {
                Int2 pos = item.Key;
                if (map.grid[pos.x, pos.y] == Tile.Empty)
                {
                    map.grid[pos.x, pos.y] = Tile.Rock;

                    // Create mock guard to analyze if we've gotten any loops.
                    // First method is we start from the beginning.
                    // Second method is we start from in front of the rock, facing it. It is significantly faster.
                    //Guard2 guard2 = new Guard2(initialGuardStart, Int2.Up); // 9472 ms
                    Guard2 guard2 = new Guard2(pos - item.Value[0], item.Value[0]); // 3365 ms

                    int result = -99;
                    do
                    {
                        // Walk once
                        result = guard2.Walk(map, true);

                        // Are we in a loop?
                        if (result == 3) totalRockCandidates.Add(pos);

                    } while (result == 1); // Repeat until we reached the end or a loop
                }

                // Reset the rock
                map.grid[pos.x, pos.y] = Tile.Empty;
            }


            // Draw all rock candidate positions
            foreach (var rock in totalRockCandidates)
            {
                Console.SetCursorPosition(rock.x, startHeightForMap + rock.y);

                var temp = guard.Path[rock][0];
                if (temp == Int2.Right)
                    TextUtilities.CFW("@Cya>");
                if (temp == Int2.Left)
                    TextUtilities.CFW("@Cya<");
                if (temp == Int2.Up)
                    TextUtilities.CFW("@Cya^");
                if (temp == Int2.Down)
                    TextUtilities.CFW("@Cyav");
            }

            // Print output
            Console.SetCursorPosition(0, startHeightForMap + map.Dim.y);
            TextUtilities.CFWLine($"\n@Gra >>> Total valid rock positions: @Yel{totalRockCandidates.Count}");
        }

        public class Guard
        {
            private Int2 position;
            private Int2 facingDirection;
            List<Int2> path = new List<Int2>();
            HashSet<Int2> distinctPlaces = new HashSet<Int2>();

            public Guard(Int2 initPosition, Int2 startingDirection)
            {
                this.position = initPosition;
                this.facingDirection = startingDirection;

                path.Add(position);
                distinctPlaces.Add(position);
            }

            public Int2 Position { get => position; }
            public Int2 FacingDirection { get => facingDirection; }

            public ReadOnlyCollection<Int2> Path => path.AsReadOnly();
            public HashSet<Int2> DistinctPlaces => distinctPlaces;

            private void Rotate90()
            {
                facingDirection = new Int2(-FacingDirection.y, FacingDirection.x);
            }

            /// <summary>
            /// Walks the guard forward.
            /// </summary>
            /// <returns>integer defining the success of the walk. 0 means it didn't succeed to move at all. 1 means success. 2 means you stepped outside the bounds.</returns>
            public int Walk(DataMap<Tile> map)
            {
                int timesTurned = 0;
                while (true)
                {
                    Int2 newPosition = Position + FacingDirection;

                    if (!map.IsInside(newPosition)) return 2;

                    switch (map.grid[newPosition.x, newPosition.y])
                    {
                        case Tile.Empty:
                            position = newPosition;

                            // Track path
                            path.Add(newPosition);

                            // Add distinct place
                            distinctPlaces.Add(newPosition);

                            return 1;
                        case Tile.Rock:
                            if (timesTurned >= 4) return 0; // If we've rotated in a full circle we've been closed in somehow
                            timesTurned++;
                            Rotate90();
                            continue; // keep looping until valid rotation is found
                        default:
                            break;
                    }
                }
            }
        }

        public class Guard2
        {
            private Int2 position;
            private Int2 facingDirection;
            Dictionary<Int2, List<Int2>> path = new Dictionary<Int2, List<Int2>>();

            public Guard2(Int2 initPosition, Int2 startingDirection)
            {
                this.position = initPosition;
                this.facingDirection = startingDirection;

                path.Add(Position, [FacingDirection]);
            }

            public Int2 Position { get => position; }
            public Int2 FacingDirection { get => facingDirection; }
            public Dictionary<Int2, List<Int2>> Path { get => path; }

            private void Rotate90()
            {
                facingDirection = new Int2(-FacingDirection.y, FacingDirection.x);

                //
            }

            /// <summary>
            /// Walks the guard forward.
            /// </summary>
            /// <returns>integer defining the success of the walk. 0 means it didn't succeed to move at all. 1 means success. 2 means you stepped outside the bounds. 3 means stuck in a loop, if bool enabled</returns>
            public int Walk(DataMap<Tile> map, bool checkIfInLoop= false)
            {
                int timesTurned = 0;
                while (true)
                {
                    Int2 nextPosition = Position + FacingDirection;

                    if (!map.IsInside(nextPosition)) return 2;

                    switch (map.grid[nextPosition.x, nextPosition.y])
                    {
                        case Tile.Empty: // If next is empty
                            // Track path
                            if (!path.TryAdd(nextPosition, [facingDirection]))
                            {
                                // This runs if we have already visited the position before

                                // If the same position and direction is detected in the dictionary
                                // 1. The guard has been here before
                                // 2. The guard was walking in the same direction
                                // 3. We have entered a loop!

                                // If next step would have us enter a loop
                                if (path[nextPosition].Contains(facingDirection)) return 3;
                                
                                // If we are not in a loop, we just add the direction as normal
                                path[nextPosition].Add(facingDirection);
                            }
                            position = nextPosition;
                            return 1;

                        case Tile.Rock: // If next is a rock
                            if (timesTurned >= 4) return 0; // If we've rotated in a full circle we've been closed in somehow
                            timesTurned++;
                            Rotate90();
                            continue; // keep looping until valid rotation is found

                        default:
                            break;
                    }
                }
            }
        }

    }

    enum Tile
    {
        Empty = '.',
        Rock = '#'
    }
}
