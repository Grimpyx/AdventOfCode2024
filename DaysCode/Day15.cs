using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.Drawing;

namespace AdventOfCode2024.Days
{
    class Day15 : IDayChallenge
    {


        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // 1509074

            var (map, robot, instructionsList) = ParseP1(DataLoader.LoadRowData(15, fullOrSimpleData));

            int cursorTopStart = Console.CursorTop;

            instructionsList.Reverse(); // required otherwise we pop from the wrong end
            Stack<Int2> instructionStack = new Stack<Int2>(instructionsList);

            DrawMap(map, robot);
            while (instructionStack.Count > 0)
            {
                Int2 instruction = instructionStack.Pop();
                robot.PerformInstructionP1(map, instruction);

                // If it is the simple dataset,
                // update each walk so you can see the steps
                if (fullOrSimpleData == DayDataType.Simple)
                {
                    Console.SetCursorPosition(0, cursorTopStart);
                    DrawMap(map, robot);
                    Thread.Sleep(75);
                }
            }

            // Draw the resulting map
            Console.SetCursorPosition(0, cursorTopStart);
            DrawMap(map, robot);

            // Calculate
            long sumOfGPS = 0;
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    char c = map.grid[x, y];
                    if (c == 'O') sumOfGPS += (100 * y) + x;
                }
            }
            TextUtilities.CFWLine($"@Gra >>> Total sum of GPS coordinates: @Yel{sumOfGPS}");

        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            var (map, robot, instructionsList) = ParseP2(DataLoader.LoadRowData(15, fullOrSimpleData));

            int cursorTopStart = Console.CursorTop;

            instructionsList.Reverse(); // required otherwise we pop from the wrong end
            Stack<Int2> instructionStack = new Stack<Int2>(instructionsList);

            DrawMap(map, robot);
            while (instructionStack.Count > 0)
            {
                Int2 instruction = instructionStack.Pop();
                robot.PerformInstructionP2(map, instruction);

                // If it is the simple dataset,
                // update each walk so you can see the steps
                if (fullOrSimpleData == DayDataType.Simple)
                {
                    Console.SetCursorPosition(0, cursorTopStart);
                    DrawMap(map, robot);
                    Thread.Sleep(75);
                }
            }

            // Draw the resulting map
            Console.SetCursorPosition(0, cursorTopStart);
            DrawMap(map, robot);

            // Calculate
            long sumOfGPS = 0;
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    char c = map.grid[x, y];
                    if (c == 'O') sumOfGPS += (100 * y) + x;
                }
            }
            TextUtilities.CFWLine($"@Gra >>> Total sum of GPS coordinates: @Yel{sumOfGPS}");
        }

        void DrawMap(DataMap<char> map, Robot robot)
        {
            int startTop = Console.CursorTop;
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    char c = map.grid[x, y];
                    string color = "@Bla";

                    if (c == 'O' || c == '[' || c == ']') color = "@Whi";
                    else if (c == '#') color = "@DGy";

                    TextUtilities.CFW(color + c);
                }
                Console.WriteLine();
            }
            int endTop = Console.CursorTop;

            Console.SetCursorPosition(robot.Position.x, robot.Position.y + startTop);
            TextUtilities.CFW("@GreX");
            Console.SetCursorPosition(0, endTop + 1);
        }

        class Robot
        {
            private Int2 position;

            public Robot(Int2 position)
            {
                this.position = position;
            }

            public Int2 Position { get => position; }

            public void PerformInstructionP1(DataMap<char> map, Int2 instruction)
            {
                Int2 nextPos = position + instruction;
                char nextChar = map.grid[nextPos.x, nextPos.y];

                // If immediately blocked, do nothing and just return.
                if (nextChar == '#')
                    return;
                // If immediately open empty, move there
                else if (nextChar == '.')
                {
                    position = nextPos;
                    return;
                }

                // else it will be an 'O'
                // Keep looking "forward" until wither '#' or '.' is found
                while (nextChar == 'O')
                {
                    nextPos += instruction;
                    nextChar = map.grid[nextPos.x, nextPos.y];
                }

                // If the next character in line is '#' we cannot move.
                // We act as if the 'O' the robot was gonna move to was a '#'
                if (nextChar == '#')
                    return;
                // If there is an empty spot, we move an 'O' onto it and walk
                // the robot liek we initially did
                else if (nextChar == '.')
                {
                    map.grid[nextPos.x, nextPos.y] = 'O';

                    // new robot position
                    position += instruction;
                    map.grid[position.x, position.y] = '.';
                }
            }

            public void PerformInstructionP2(DataMap<char> map, Int2 instruction)
            {
                Int2 nextPos = position + instruction;
                char nextChar = map.grid[nextPos.x, nextPos.y];

                // If immediately blocked, do nothing and just return.
                if (nextChar == '#')
                    return;
                // If immediately open empty, move there
                else if (nextChar == '.')
                {
                    position = nextPos;
                    return;
                }




                // Handles left right movement of boxes "[]"
                if (instruction == Int2.Right || instruction == Int2.Left)
                {
                    while (nextChar == '[' || nextChar == ']')
                    {
                        nextPos += instruction;
                        nextChar = map.grid[nextPos.x, nextPos.y];
                    }

                    // If the next character in line is '#' we cannot move.
                    // We act as if the 'O' the robot was gonna move to was a '#'
                    if (nextChar == '#')
                        return;
                    // If there is an empty spot, we move the entire contraption one step
                    else if (nextChar == '.')
                    {
                        int diff = nextPos.x - position.x;
                        int dist = Math.Abs(diff);
                        int sign = diff / dist;

                        for (int x = 0; x < dist; x++)
                        {
                            Int2 toChange = nextPos - sign * x * Int2.Right;
                            Int2 from = new Int2(toChange.x - sign * 1, toChange.y);
                            char fromCharr = map.grid[from.x, from.y];
                            map.grid[toChange.x, toChange.y] = fromCharr;
                        }

                        // new robot position
                        position += instruction;
                        map.grid[position.x, position.y] = '.';
                    }
                }
                .
                // Up and down movement of boxes
                (Int2 left, Int2 right) box;
                if (nextChar == '[')
                {
                    box.left = nextPos;
                    box.right = nextPos + Int2.Right;
                }
                else if (nextChar == ']')
                {
                    box.left = nextPos + Int2.Left;
                    box.right = nextPos;
                }




            }

        }

        (DataMap<char> map, Robot robot, List<Int2> instructions) ParseP1(string[] rows)
        {
            Robot robot = new Robot(Int2.Zero); // lazy zzz

            List<Int2> instructions = new List<Int2>();

            // Divide the data into two groups, the rows for the map; and the rows for the instructions.
            List<string> instructionsStrings = new List<string>();
            List<string> mapStrings = new List<string>();
            int i = 0;
            while (true)
            {
                if (rows[i] == "") break;

                mapStrings.Add(rows[i]);
                i++;
            }
            i++;
            for (int l = i; l < rows.Length; l++)
            {
                instructionsStrings.Add(rows[l]);
            }

            // Interpret the map part
            char[,] grid = new char[mapStrings[0].Length, mapStrings.Count];
            for (int y = 0; y < mapStrings.Count; y++)
            {
                for (int x = 0; x < mapStrings[0].Length; x++)
                {
                    char c = mapStrings[y][x];
                    if (c == '@')
                    {
                        grid[x, y] = '.';
                        robot = new Robot(new Int2(x,y));
                    }
                    else grid[x,y] = c;
                }
            }
            DataMap<char> map = new DataMap<char>(grid);

            // Interpret the instructions part
            foreach (var str in instructionsStrings)
            {
                for (int j = 0; j < str.Length; j++)
                {
                    char c = str[j];

                    instructions.Add(c switch
                    {
                        '^' => Int2.Up,
                        'v' => Int2.Down,
                        '<' => Int2.Left,
                        '>' => Int2.Right,
                        _ => Int2.Zero
                    });
                }
            }

            // Return
            return (map, robot, instructions);
        }

        (DataMap<char> map, Robot robot, List<Int2> instructions) ParseP2(string[] rows)
        {
            Robot robot = new Robot(Int2.Zero); // lazy zzz

            List<Int2> instructions = new List<Int2>();

            // Divide the data into two groups, the rows for the map; and the rows for the instructions.
            List<string> instructionsStrings = new List<string>();
            List<string> mapStrings = new List<string>();
            int i = 0;
            while (true)
            {
                string row = rows[i];
                if (row == "") break;

                string newRow = "";
                foreach (char c in row)
                {
                    if (c == '#') newRow += "##";
                    else if (c == 'O') newRow += "[]";
                    else if (c == '.') newRow += "..";
                    else if (c == '@') newRow += "@.";
                    else throw new Exception();
                }

                mapStrings.Add(newRow);
                i++;
            }
            i++;
            for (int l = i; l < rows.Length; l++)
            {
                instructionsStrings.Add(rows[l]);
            }

            // Interpret the map part
            char[,] grid = new char[mapStrings[0].Length, mapStrings.Count];
            for (int y = 0; y < mapStrings.Count; y++)
            {
                for (int x = 0; x < mapStrings[0].Length; x++)
                {
                    char c = mapStrings[y][x];
                    if (c == '@')
                    {
                        grid[x, y] = '.';
                        robot = new Robot(new Int2(x, y));
                    }
                    else grid[x, y] = c;
                }
            }
            DataMap<char> map = new DataMap<char>(grid);

            // Interpret the instructions part
            foreach (var str in instructionsStrings)
            {
                for (int j = 0; j < str.Length; j++)
                {
                    char c = str[j];

                    instructions.Add(c switch
                    {
                        '^' => Int2.Up,
                        'v' => Int2.Down,
                        '<' => Int2.Left,
                        '>' => Int2.Right,
                        _ => Int2.Zero
                    });
                }
            }

            // Return
            return (map, robot, instructions);
        }
    }
}
