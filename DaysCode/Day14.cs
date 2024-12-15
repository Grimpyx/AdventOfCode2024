using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using static AdventOfCode2024.Days.Day14;

namespace AdventOfCode2024.Days
{
    class Day14 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            Int2 maxDimensions;

            if (fullOrSimpleData == DayDataType.Simple)
                maxDimensions = new Int2(11, 7);
            else maxDimensions = new Int2(101, 103);

            List<Robot> robots = Parse(DataLoader.LoadRowData(14, fullOrSimpleData), maxDimensions);


            int topLeft = 0, topRight = 0, bottomLeft = 0, bottomRight = 0;
            foreach (var r in robots)
            {
                r.Walk(100);

                // Right half
                if (r.Position.x > maxDimensions.x / 2)
                {
                    // Bottom right quadrant
                    if (r.Position.y > maxDimensions.y / 2)
                        bottomRight++;
                    // Top right quadrant
                    else if (r.Position.y < maxDimensions.y / 2)
                        topRight++;
                }
                // Left half
                else if (r.Position.x < maxDimensions.x / 2)
                {
                    // Bottom left quadrant
                    if (r.Position.y > maxDimensions.y / 2)
                        bottomLeft++;
                    // Top left quadrant
                    else if (r.Position.y < maxDimensions.y / 2)
                        topLeft++;
                }
            }
            int safetyFactor = topLeft * topRight * bottomLeft * bottomRight;

            TextUtilities.CFWLine($"@Gra >>> Safety factor: @Yel{safetyFactor}");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // Eeeh... what exactly does a chrismas tree look like?
            // Since we don't really know, we must make assumptions:
            // 1. It's vertically oriented, and is symmetrical. We MIGHT get a
            // christmas tree if the left and right sides are completely symmetrical
            // I tried this but it didn't work. It seems the christmas tree is not
            // symmetrically placed on the map.
            // 2. A christmas tree seems to be connected to multiple other points.
            // So if we select a robot and continuially walk along its neighbours
            // we might have found a cohesive picture. (this worked)

            Int2 maxDimensions = new Int2(101, 103);
            List<Robot> robots = Parse(DataLoader.LoadRowData(14, DayDataType.Full), maxDimensions); // We only have the full dataset that this works for

            // This datamap represents how many robots are in one spot
            // Starts with no robot anywhere
            DataMap<int> robotPositions = new DataMap<int>(maxDimensions, 0);
            foreach (var robot in robots)
            {
                robotPositions.grid[robot.Position.x, robot.Position.y]++;
            }

            // Write grid
            int cursorStart = Console.CursorTop;
            WriteGrid(robotPositions.grid);

            int stepCounter = 0; // counts seconds

            // Needed for tracking the adjacently connected robots
            HashSet<Int2> visited = new HashSet<Int2>();
            Stack<Int2> positionStack = new Stack<Int2>();

            while (true)
            {
                // Move all robots
                foreach (var robot in robots)
                {
                    robotPositions.grid[robot.Position.x, robot.Position.y]--;  // Remove old position
                    
                    Int2 nextPos = robot.Walk();

                    robotPositions.grid[nextPos.x, nextPos.y]++;                // Add new position
                }
                stepCounter++;

                // See if it has many robots next to eachother
                // If we have a lot of robots connected, 
                visited.Clear();
                positionStack.Clear();
                positionStack.Push(robots[0].Position);
                int connectedAmount = 0;

                while (positionStack.Count > 0)
                {
                    Int2 pos = positionStack.Pop();
                    visited.Add(pos);

                    connectedAmount++;

                    List<Int2> adjacents = robotPositions.GetAllAdjacentPositions(pos);
                    foreach (Int2 adjacent in adjacents)
                    {
                        int adjacentValue = robotPositions.grid[adjacent.x, adjacent.y];

                        // Queue all valid adjacent positions
                        if (adjacentValue > 0 && !visited.Contains(adjacent))
                        {
                            positionStack.Push(adjacent);
                        }
                    }
                }

                // 20 consecutively connected robots is enough to find the christmas tree
                if (connectedAmount > 20)
                    break;

                /* THIS DIDNT WORK BECAUSE THE PICTURE WASN'T A PERFECT MIRROR
                // See if it is mirrored
                bool isMirrored = true;
                foreach (var robot in robots)
                {
                    Int2 pos = robot.Position;  // Remove old position
                    Int2 mirroredPos = new Int2(robotPositions.Dim.x - pos.x - 1, pos.y);

                    int valueRobot = robotPositions.grid[pos.x,pos.y];
                    int valueMirrored = robotPositions.grid[mirroredPos.x, mirroredPos.y];

                    if (valueRobot != valueMirrored) //robotPositions.grid[mirroredPos.x, mirroredPos.y] < 1)
                    {
                        isMirrored = false;
                        break;
                    }
                }*/

            }

            // Draw output
            Console.SetCursorPosition(0, cursorStart);
            WriteGrid(robotPositions.grid);
            int cursorEnd = Console.CursorTop;
            foreach (Int2 p in visited)
            {
                Console.SetCursorPosition(p.x, p.y + cursorStart);
                TextUtilities.CFW("@GreX@Gra");
            }
            Console.SetCursorPosition(0, cursorEnd);
            Console.WriteLine();

            TextUtilities.CFWLine($"@Gra >>> Total seconds elapsed from start: @Yel{stepCounter}@Gra");
        }

        void WriteGrid(int[,] grid)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    int value = grid[x, y];
                    string msg;
                    if (value == 0) msg = "@DGy.";
                    else if (value > 9) msg = "@RedX";
                    else msg = "@DCy" + grid[x, y].ToString();

                    TextUtilities.CFW(msg);
                }
                Console.WriteLine();
            }
        }

        List<Robot> Parse(string[] data, Int2 maxDimensions)
        {
            List<Robot> robotList = new List<Robot>();
            foreach (var row in data)
            {
                var dataSplit = row.Split(' ');
                var leftData = dataSplit[0][2..].Split(',');
                var rightData = dataSplit[1][2..].Split(',');
                Int2 p = new Int2(int.Parse(leftData[0]), int.Parse(leftData[1]));
                Int2 v = new Int2(int.Parse(rightData[0]), int.Parse(rightData[1]));

                robotList.Add(new Robot(p, v, maxDimensions));
            }
            return robotList;
        }

        public class Robot
        {
            private Int2 position;
            private Int2 velocity;
            private Int2 maxDimensions;

            public Int2 Position { get => position; }

            public Robot(Int2 position, Int2 velocity, Int2 maxDimensions)
            {
                this.position = position;
                this.velocity = velocity;
                this.maxDimensions = maxDimensions;
            }

            // Returns the new position
            public Int2 Walk(int times = 1)
            {
                // To ensure the positions wrap, we can cleverly just use modulus

                int x = (position.x + times * velocity.x) % maxDimensions.x;
                if (x < 0) x += maxDimensions.x;
                int y = (position.y + times * velocity.y) % maxDimensions.y;
                if (y < 0) y += maxDimensions.y;
                position = new Int2(x, y);
                return position;
            }

            public override string ToString()
            {
                return position.ToString()!;
            }
        }
    }
}
