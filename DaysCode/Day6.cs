using AdventOfCode2024.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('G');

                v -= guard.FacingDirection;
                Console.SetCursorPosition(v.x, startHeightForMap + v.y);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write('X');

                if (!instantDraw) Thread.Sleep(15);
            }
            Console.SetCursorPosition(0, startHeightForMap + map.Dim.y);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($" >>> Total path: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(guard.Path.Count);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($" >>> Distinct:   ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(guard.DistinctPlaces.Count);

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

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
    }

    enum Tile
    {
        Empty = '.',
        Rock = '#'
    }
}
