using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day21 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // Too high 94758
            // 94426 correct.

            string[] rowData = DataLoader.LoadRowData(21, fullOrSimpleData);

            List<Keypad> keypads = new List<Keypad>()
            {
                new NumericKeypad(),
                new DirectionalKeypad(),
                new DirectionalKeypad()
            };

            //List<char> shortest = new List<char>();
            List<long> complexity = new List<long>();
            foreach (var s in rowData)
            {
                List<List<char>> permutations = [new List<char>(s)];


                for (int keypadIndex = 0; keypadIndex < keypads.Count; keypadIndex++)
                {

                    List<List<char>> nextPerms = new List<List<char>>();
                    foreach (var existingPermutation in permutations)
                    {
                        // InstructionsToPressKeys returns a list of directional chars
                        nextPerms.AddRange(keypads[keypadIndex].InstructionsToPressKeys(existingPermutation));
                    }
                    permutations = nextPerms;
                }
                complexity.Add(permutations.Select(x => x.Count).Min() * long.Parse(s[..^1]));
                Console.WriteLine(complexity[^1]);
            }
            TextUtilities.CFWLine($"\n@Gra >>> Total complexity: @Yel{complexity.Sum()}");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }

        class Keypad
        {
            protected Int2 currentPosition;
            protected Int2 unallowedPosition;
            protected Dictionary<Int2, char> posToKey = new Dictionary<Int2, char>();
            protected Dictionary<char, Int2> keyToPos = new Dictionary<char, Int2>();

            public Keypad(Int2 currentPosition, Dictionary<Int2, char> posToKey, Dictionary<char, Int2> keyToPos)
            {
                this.currentPosition = currentPosition;
                unallowedPosition = -1 * Int2.One;
                this.posToKey = posToKey;
                this.keyToPos = keyToPos;
            }
            protected Keypad() { }

            public List<List<char>> InstructionsToPressKeys(List<char> keys)
            {
                List<List<char>> allPermutations = new List<List<char>>(InstructionsToPressKey(keys[0]));

                for (int i = 1; i < keys.Count; i++)
                {
                    char c = keys[i];
                    List<List<char>> nextPermutations = new List<List<char>>();

                    List<List<char>> waysToPressKey = InstructionsToPressKey(c);
                    int nrOfways = waysToPressKey.Count;
                    foreach (List<char> way in waysToPressKey)
                    {
                        foreach (var currentWay in allPermutations)
                        {
                            nextPermutations.Add([.. currentWay, .. way]);
                        }
                    }
                    allPermutations = nextPermutations;
                }

                return allPermutations;
            }

            public List<List<char>> InstructionsToPressKey(char key)
            {
                Int2 posAim = keyToPos[key];
                Int2 diff = posAim - currentPosition;

                Int2 hDir = Int2.Zero;
                Int2 vDir = Int2.Zero;
                if (diff.x > 0) hDir = Int2.Right;
                if (diff.x < 0) hDir = Int2.Left;
                if (diff.y > 0) vDir = Int2.Down;
                if (diff.y < 0) vDir = Int2.Up;

                Queue<List<(Int2 pos, Int2 dir)>> positionQueue = new Queue<List<(Int2 pos, Int2 dir)>>();
                List<List<(Int2 pos, Int2 dir)>> validPathsToGoal = new List<List<(Int2 pos, Int2 dir)>>();
                positionQueue.Enqueue([(currentPosition, Int2.Zero)]);
                while (positionQueue.Count > 0)
                {
                    List<(Int2 pos, Int2 dir)> dequeue = positionQueue.Dequeue();
                    Int2 lastPos = dequeue[^1].pos;

                    // Found a path
                    if (lastPos == posAim)
                    {
                        validPathsToGoal.Add(dequeue);
                        continue;
                    }

                    // Hit unallowed spot
                    if (lastPos == unallowedPosition)
                        continue;

                    // If outside bounds
                    if (!posToKey.ContainsKey(lastPos))
                        continue;

                    // Queue the contents of the current path + next step (previous position + horizontal or vertical)
                    if (hDir.x != 0)
                    {
                        List<(Int2 pos, Int2 dir)> nextStepHorizontal = new List<(Int2 pos, Int2 dir)>([.. dequeue, (lastPos + hDir, hDir)]);
                        positionQueue.Enqueue(nextStepHorizontal);
                    }

                    if (vDir.y != 0)
                    {
                        List<(Int2 pos, Int2 dir)> nextStepVertical = new List<(Int2 pos, Int2 dir)>([.. dequeue, (lastPos + vDir, vDir)]);
                        positionQueue.Enqueue(nextStepVertical);
                    }
                }

                // Update position to where we wanted to go
                // agnostic to the path we took
                currentPosition = posAim;

                // Convert list of positions
                // to list of characters
                List<List<char>> directionList = validPathsToGoal.Select(innerList => innerList.Skip(1).Select(listElement => Int2ToChar(listElement.dir)).ToList()).ToList();
                directionList.ForEach(x => x.Add('A'));

                return directionList;
            }




            public char Int2ToChar(Int2 v)
            {
                if (v == Int2.Right) return '>';
                if (v == Int2.Left) return '<';
                if (v == Int2.Up) return '^';
                if (v == Int2.Down) return 'v';
                else return 'E';
            }
            public Int2 CharToInt2(char c)
            {
                if (c == '>') return Int2.Right;
                if (c == '<') return Int2.Left;
                if (c == '^') return Int2.Up;
                if (c == 'v') return Int2.Down;
                else return Int2.Zero;
            }
        }

        class NumericKeypad : Keypad
        {
            public NumericKeypad()
            {
                currentPosition = new Int2(2, 3); // bottom right corner

                unallowedPosition = new Int2(0, 3);

                posToKey = new Dictionary<Int2, char>()
                {
                    { new Int2(0,0), '7' },
                    { new Int2(1,0), '8' },
                    { new Int2(2,0), '9' },
                    { new Int2(0,1), '4' },
                    { new Int2(1,1), '5' },
                    { new Int2(2,1), '6' },
                    { new Int2(0,2), '1' },
                    { new Int2(1,2), '2' },
                    { new Int2(2,2), '3' },
                    { new Int2(1,3), '0' },
                    { new Int2(2,3), 'A' }
                };

                keyToPos = new Dictionary<char, Int2>()
                {
                    { '7', new Int2(0,0) },
                    { '8', new Int2(1,0) },
                    { '9', new Int2(2,0) },
                    { '4', new Int2(0,1) },
                    { '5', new Int2(1,1) },
                    { '6', new Int2(2,1) },
                    { '1', new Int2(0,2) },
                    { '2', new Int2(1,2) },
                    { '3', new Int2(2,2) },
                    { '0', new Int2(1,3) },
                    { 'A', new Int2(2,3) }
                };
            }
        }

        class DirectionalKeypad : Keypad
        {
            public DirectionalKeypad()
            {
                currentPosition = new Int2(2, 0); // bottom right corner

                unallowedPosition = new Int2(0, 0);

                posToKey = new Dictionary<Int2, char>()
                {
                    { new Int2(1,0), '^' },
                    { new Int2(2,0), 'A' },
                    { new Int2(0,1), '<' },
                    { new Int2(1,1), 'v' },
                    { new Int2(2,1), '>' }
                };

                keyToPos = new Dictionary<char, Int2>()
                {
                    { '^', new Int2(1,0) },
                    { 'A', new Int2(2,0) },
                    { '<', new Int2(0,1) },
                    { 'v', new Int2(1,1) },
                    { '>', new Int2(2,1) }
                };
            }
        }
    }
}
