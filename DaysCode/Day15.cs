using AdventOfCode2024.Utilities;
using System.ComponentModel.DataAnnotations.Schema;
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
            // 1465522 too low!!
            // 1491655 too low!!
            // 1521453 correct!
            // Problem I had was that my multiple box algorithm just added all the boxes to move into a list.
            // But when it was time to move them, there was no guarantee that the next box to move was had a clear path above it.
            // I remade the algorithm to take more logical STEPS:
            // We step up once from the start box and add all box to a list. All these boxes can be moved at the same time.
            // That list of boxes, is added to another list that tracks groups of boxes that can be moved together.
            // By iterating backward through the group we move the boxes furthest away first.

            var (map, robot, instructionsList) = ParseP2(DataLoader.LoadRowData(15, fullOrSimpleData));

            int cursorTopStart = Console.CursorTop;

            instructionsList.Reverse(); // required otherwise we pop from the wrong end
            Stack<Int2> instructionStack = new Stack<Int2>(instructionsList);

            int boxInTheBeginning = 0;
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    if (map.grid[x,y] == '.')
                        boxInTheBeginning++;
                }
            }

            DrawMap(map, robot);
            int instructionsPerformed = 0; // Tracks when to draw
            while (instructionStack.Count > 0)
            {
                Int2 instruction = instructionStack.Pop();

                // Perform instruction
                robot.PerformInstructionP2(map, instruction);

                // If it is the simple dataset,
                // update each walk so you can see the steps
                if (fullOrSimpleData == DayDataType.Simple)
                {
                    Thread.Sleep(200);
                    Console.SetCursorPosition(0, cursorTopStart);
                    DrawMap(map, robot);
                }
                else
                {
                    // For the longer dataset, only draw the map every 400th instruction
                    if (instructionsPerformed % 400 == 0)
                    {
                        Console.SetCursorPosition(0, cursorTopStart);
                        DrawMap(map, robot);
                    }
                }
                instructionsPerformed++;
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
                    if (c == '[') sumOfGPS += (100 * y) + x;
                }
            }
            TextUtilities.CFWLine($"@Gra >>> Total sum of GPS coordinates: @Yel{sumOfGPS}");
        }

        void DrawMap(DataMap<char> map, Robot robot)
        {
            int startTop = Console.CursorTop;
            string s = "";
            for (int y = 0; y < map.Dim.y; y++)
            {
                for (int x = 0; x < map.Dim.x; x++)
                {
                    char c = map.grid[x, y];
                    string color = "@Bla";

                    if (c == 'O' || c == '[' || c == ']') color = "@Whi";
                    else if (c == '#') color = "@DGy";

                    s += color + c;
                }
                s += "\n";
            }
            TextUtilities.CFWLine(s);

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
                // the next
                Int2 robotInstructionPosition = position + instruction;
                char robotInstructionChar = map.grid[robotInstructionPosition.x, robotInstructionPosition.y];

                // If immediately blocked, do nothing and just return.
                if (robotInstructionChar == '#')
                    return;
                // If immediately open empty, move there
                else if (robotInstructionChar == '.')
                {
                    position = robotInstructionPosition;
                    return;
                }


                // Handles left right movement of boxes "[]"
                if (instruction == Int2.Right || instruction == Int2.Left)
                {
                    // Step in the direction of the instruction until either a stop '#' or empty space '.' is found.
                    while (robotInstructionChar == '[' || robotInstructionChar == ']')
                    {
                        robotInstructionPosition += instruction;
                        robotInstructionChar = map.grid[robotInstructionPosition.x, robotInstructionPosition.y];
                    }

                    // If the next character in line is '#' we cannot move.
                    // We wont move anything, not even the robot. We return.
                    if (robotInstructionChar == '#')
                        return;
                    // If there is an empty spot, we move the entire contraption one step
                    else if (robotInstructionChar == '.')
                    {
                        // Depending on if we're moving left or right the logic is slightly different.
                        // Now when I write the comments, the obvious way is to just use the instruction... But this works :)
                        int diff = robotInstructionPosition.x - position.x;
                        int dist = Math.Abs(diff);
                        int sign = diff / dist;

                        for (int x = 0; x < dist; x++)
                        {
                            Int2 toChange = robotInstructionPosition - sign * x * Int2.Right;
                            Int2 from = new Int2(toChange.x - sign * 1, toChange.y);
                            char fromCharr = map.grid[from.x, from.y];
                            map.grid[toChange.x, toChange.y] = fromCharr;
                        }

                        // Move robot
                        position += instruction;
                    }
                }
                else if (instruction == Int2.Up || instruction == Int2.Down)
                {
                    // Up and down movement of boxes

                    (Int2 left, Int2 right) firstBox = (Int2.Zero, Int2.Zero);

                    // Establish the first box
                    if (robotInstructionChar == '[')
                    {
                        firstBox.left = robotInstructionPosition;
                        firstBox.right = robotInstructionPosition + Int2.Right;
                    }
                    else if (robotInstructionChar == ']')
                    {
                        firstBox.left = robotInstructionPosition + Int2.Left;
                        firstBox.right = robotInstructionPosition;
                    }


                    // This contains all the groups of boxes.
                    //  [][][] <- allBoxes[2], 3 boxes
                    //   [][]  <- allBoxes[1], 2 boxes
                    //    []   <- allBoxes[0], 1 box
                    //    @    <- robot
                    List<List<(Int2 left, Int2 right)>> allBoxes = new List<List<(Int2 left, Int2 right)>>();

                    // Contains all boxes in one group.
                    Stack<List<(Int2 left, Int2 right)>> boxStack = new Stack<List<(Int2 left, Int2 right)>>();

                    // First group is just the one first box
                    boxStack.Push([firstBox]);
                    allBoxes.Add([firstBox]);

                    List<Int2> nextPositionsLeft, nextPositionsRight;
                    List<char> nextLeftChar, nextRightChar;

                    do
                    {
                        var boxGroup = boxStack.Pop();

                        // For all boxes in the group, this is the next positions of their left respective right sides, in the direction of the instruction
                        nextPositionsLeft = boxGroup.Select(x => x.left + instruction).ToList();
                        nextPositionsRight = boxGroup.Select(x => x.right + instruction).ToList();

                        // The chars tied to the positions above
                        nextLeftChar = nextPositionsLeft.Select(box => map.grid[box.x, box.y]).ToList();
                        nextRightChar = nextPositionsRight.Select(box => map.grid[box.x, box.y]).ToList();

                        // If any next chars are '#' we've hit a wall and we cannot push.
                        if (nextLeftChar.Contains('#') || nextRightChar.Contains('#'))
                            return;

                        // Look in front of all boxes in this group.
                        // We add all new boxes found in the direction of the instruction to the below list.
                        // At the end we add all boxes in the below list to allBoxes and the box stack so we can
                        // track what boxgroup we found and what we need to keep searching from.
                        List<(Int2 left, Int2 right)> boxesToAddThisIteration = new List<(Int2 left, Int2 right)>();
                        for (int i = 0; i < nextLeftChar.Count; i++)
                        {
                            Int2 pl = nextPositionsLeft[i];
                            char cl = nextLeftChar[i];

                            Int2 pr = nextPositionsRight[i];
                            char cr = nextRightChar[i];

                            // Two cases: either you have 1 box, or 2 boxes.
                            // If one box, track if it is offset to neither direction, to the left, or to the right.

                            Int2 offset = Int2.Zero;
                            bool doubleBox = false;
                            
                            if (cl == '.' && cr == '[') // If single box to the right
                                offset = Int2.Right;
                            else if (cl == ']' && cr == '.') // If single box to the left
                                offset = Int2.Left;
                            else if (cl == ']' && cr == '[') // If double box
                                doubleBox = true;
                            else if (cl == '.' && cr == '.') // If clear in front, dont add anything
                                continue;

                            if (doubleBox)
                            {
                                (Int2 left, Int2 right) leftBox = (pl + Int2.Left, pr + Int2.Left);
                                (Int2 left, Int2 right) rightBox = (pl + Int2.Right, pr + Int2.Right);

                                // In some cases we might add a box twice, so we ensure there are no duplicates
                                if (!boxesToAddThisIteration.Contains(leftBox))
                                    boxesToAddThisIteration.Add(leftBox);
                                if (!boxesToAddThisIteration.Contains(rightBox))
                                    boxesToAddThisIteration.Add(rightBox);
                            }
                            else
                            {
                                (Int2 left, Int2 right) nextBox = (pl + offset, pr + offset);

                                // In some cases we might add a box twice, so we ensure there are no duplicates
                                if (!boxesToAddThisIteration.Contains(nextBox))
                                    boxesToAddThisIteration.Add(nextBox);
                            }
                        }
                        if (boxesToAddThisIteration.Count > 0)
                        {
                            allBoxes.Add(boxesToAddThisIteration);
                            boxStack.Push(boxesToAddThisIteration);
                        }
                    } while (boxStack.Count > 0);


                    // We end the above do-while when we've found all boxes that exist.

                    for (int boxGroupID = allBoxes.Count-1; boxGroupID >= 0; boxGroupID--)
                    {
                        // Grab the next group of boxes
                        var boxGroup = allBoxes[boxGroupID];

                        for (int boxID = 0; boxID < boxGroup.Count; boxID++)
                        {
                            // Grab the next box in the current box group
                            var box = boxGroup[boxID];

                            Int2 leftTo = box.left + instruction;
                            Int2 rightTo = box.right + instruction;

                            // Copy map values to the moved position
                            map.grid[leftTo.x, leftTo.y] = map.grid[box.left.x, box.left.y];
                            map.grid[rightTo.x, rightTo.y] = map.grid[box.right.x, box.right.y];

                            // Clear the previous position
                            map.grid[box.left.x, box.left.y] = '.';
                            map.grid[box.right.x, box.right.y] = '.';
                        }
                    }
                    position += instruction;
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
