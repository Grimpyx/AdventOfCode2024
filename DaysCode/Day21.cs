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
            TextUtilities.CFWLine($"\n@Gra >>> Lowest total complexity: @Yel{complexity.Sum()}");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // Bruh this is hard
            string[] rowData = DataLoader.LoadRowData(21, fullOrSimpleData);

            NumericKeypad keypadNumeric = new NumericKeypad();
            DirectionalKeypad keypadDirectional = new DirectionalKeypad();

            long complexity = 0;
            foreach (var code in rowData)
            {
                // Because I have two different keypad objects, we need to first translate the resulting moves
                // that will be entered on the keypad in the end.
                // When we recursively act on 25 layers of robots we will instead supply 24 layers (because we did the first layer here)
                List<List<char>> numericMoves =  keypadNumeric.InstructionsToPressKeys(new List<char>(code));

                TextUtilities.CFWLine($"@RedCode@Gra: @Yel{code}@Gra");

                // Calculate cost of each
                long codeCost = long.MaxValue;
                foreach (var numericMove in numericMoves)
                {
                    TextUtilities.CFWLine("@Red > @GraFirst layer permutation: [@DYe" + string.Concat(numericMove) + "@Gra]");
                    long cost = 0;
                    for (int i = 0; i < numericMove.Count; i++)
                    {
                        int layers = 24; // We have already calculated the first layer
                        char from = i == 0 ? 'A' : numericMove[i - 1];
                        long lowestCostForChar = keypadDirectional.Cost(from, numericMove[i], layers, new Dictionary<(char start, char destination, int robotDepth), long>());
                        cost += lowestCostForChar;
                    }

                    TextUtilities.CFWLine("@Red > @GraPermutation cost: @Yel" + cost + "\n");
                    if (cost < codeCost)
                        codeCost = cost;
                }
                TextUtilities.CFWLine("@Gre Lowest cost among all permutations: @Yel" + codeCost + "\n");

                long complexityToAdd = codeCost * long.Parse(code[..^1]);
                complexity += complexityToAdd;
            }
            TextUtilities.CFWLine($"\n@Gra >>> Lowest total complexity: @Yel{complexity}");
        }
    }

    class Keypad
    {
        protected Int2 unallowedPosition;
        protected Dictionary<Int2, char> posToKey = new Dictionary<Int2, char>();
        protected Dictionary<char, Int2> keyToPos = new Dictionary<char, Int2>();

        public Keypad(Int2 currentPosition, Dictionary<Int2, char> posToKey, Dictionary<char, Int2> keyToPos)
        {
            unallowedPosition = -1 * Int2.One;
            this.posToKey = posToKey;
            this.keyToPos = keyToPos;
        }
        protected Keypad() { }

        public virtual List<List<char>> InstructionsToPressKeys(List<char> keys)
        {
            List<List<char>> allPermutations = new List<List<char>>(InstructionsToPressKey('A', keys[0]));

            for (int i = 1; i < keys.Count; i++)
            {
                List<List<char>> nextPermutations = new List<List<char>>();

                List<List<char>> waysToPressKey = InstructionsToPressKey(keys[i - 1], keys[i]);
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
        public List<List<char>> InstructionsToPressKey(char fromKey, char toKey)
        {
            // This function finds all ways to reach a key.

            Int2 posAim = keyToPos[toKey];
            Int2 posStart = keyToPos[fromKey];
            Int2 diff = posAim - posStart;

            // Establish if we walk left or right, and up or down to reach 
            // the goal. This is done using the difference.
            Int2 hDir = Int2.Zero;
            Int2 vDir = Int2.Zero;
            if (diff.x > 0) hDir = Int2.Right;
            if (diff.x < 0) hDir = Int2.Left;
            if (diff.y > 0) vDir = Int2.Down;
            if (diff.y < 0) vDir = Int2.Up;

            // Queue positions
            Queue<List<(Int2 pos, Int2 dir)>> positionQueue = new Queue<List<(Int2 pos, Int2 dir)>>();
            List<List<(Int2 pos, Int2 dir)>> validPathsToGoal = new List<List<(Int2 pos, Int2 dir)>>();
            positionQueue.Enqueue([(posStart, Int2.Zero)]);
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
                // Horizontal
                if (hDir.x != 0)
                {
                    // Create a new list to queue with the next position included
                    List<(Int2 pos, Int2 dir)> nextStepHorizontal = new List<(Int2 pos, Int2 dir)>([.. dequeue, (lastPos + hDir, hDir)]);
                    positionQueue.Enqueue(nextStepHorizontal);
                }

                // Vertical
                if (vDir.y != 0)
                {
                    // Create a new list to queue with the next position included
                    List<(Int2 pos, Int2 dir)> nextStepVertical = new List<(Int2 pos, Int2 dir)>([.. dequeue, (lastPos + vDir, vDir)]);
                    positionQueue.Enqueue(nextStepVertical);
                }
            }

            // Convert list of positions
            // to list of characters
            List<List<char>> directionList = validPathsToGoal
                .Select(innerList => innerList
                .Skip(1)
                .Select(listElement => Int2ToChar(listElement.dir)).ToList()).ToList();
            directionList.ForEach(x => x.Add('A'));

            return directionList;
        }

        // Converts direction to char
        public char Int2ToChar(Int2 v)
        {
            if (v == Int2.Right) return '>';
            if (v == Int2.Left) return '<';
            if (v == Int2.Up) return '^';
            if (v == Int2.Down) return 'v';
            else return 'E';
        }

        // Converts char to direction
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
            // The empty space, kinda unnecessary
            unallowedPosition = new Int2(0, 3);

            // All available positions
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

            // All available positions
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
            // The empty space, kinda unnecessary
            unallowedPosition = new Int2(0, 0);

            // All available positions
            posToKey = new Dictionary<Int2, char>()
            {
                { new Int2(1,0), '^' },
                { new Int2(2,0), 'A' },
                { new Int2(0,1), '<' },
                { new Int2(1,1), 'v' },
                { new Int2(2,1), '>' }
            };

            // All available positions
            keyToPos = new Dictionary<char, Int2>()
            {
                { '^', new Int2(1,0) },
                { 'A', new Int2(2,0) },
                { '<', new Int2(0,1) },
                { 'v', new Int2(1,1) },
                { '>', new Int2(2,1) }
            };
        }

        // Thanks Perfect-Standard-230
        // https://www.reddit.com/r/adventofcode/comments/1hj2odw/comment/m3flzcu/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
        // https://pastebin.com/Y8YHSWSs

        // And also 
        // https://www.youtube.com/watch?v=q5I6ZvJmHEo
        // https://github.com/womogenes/AoC-2024-Solutions/tree/main/day_21
        public long Cost(char start, char destination, int robotDepth, Dictionary<(char start, char destination, int robotDepth), long> cache)
        {
            if (cache.TryGetValue((start, destination, robotDepth), out long valueInCache))
                return valueInCache;

            // Lowest value starts at max value, and later we select the lowest among all options
            long lowestValue = long.MaxValue;

            // Get all moves (and permutations) that can lead from one point (start) to a destination
            List<List<char>> moves = InstructionsToPressKey(start, destination);
            
            // If depth is 0, we just grab the shortest path's length
            if (robotDepth == 0)
                return moves.Select(x => x.Count).Min();

            // We find the shortest cost all the way to depth 0
            foreach (List<char> move in moves)
            {
                long sum = 0;
                for (int i = 0; i < move.Count; i++)
                {
                    char from = i == 0 ? 'A' : move[i - 1];
                    sum += Cost(from, move[i], robotDepth - 1, cache); // Sum the cost of all moves
                }

                if (sum < lowestValue)
                {
                    lowestValue = sum;
                }
            }

            // Add memoization, making next quicker
            cache.Add((start, destination, robotDepth), lowestValue);
            return lowestValue;
        }
    }


}
