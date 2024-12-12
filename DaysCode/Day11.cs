using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.Diagnostics.SymbolStore;

namespace AdventOfCode2024.Days
{
    class Day11 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // Correct answer: 197157

            LinkedList<long> stoneList = Parse(DataLoader.LoadAllData(11, fullOrSimpleData));

            TextUtilities.CFWLine("@GraThe first @Yel6 @Grablinks:");
            for (int i = 0; i < 25; i++)
            {
                // Draw
                if (i < 7)
                {
                    foreach (var item in stoneList)
                    {
                        TextUtilities.CFW("@Yel" + item.ToString() + " @Gra");
                    }
                    Console.WriteLine('\n' + new string('-', Console.WindowWidth - 1));
                }

                // Perform blink for each stone
                LinkedListNode<long> stone = stoneList.First!;
                while (stone != null)
                {
                    long value = stone.Value;
                    if (value == 0) // Become 1
                    {
                        stone.Value = 1;
                    }
                    else
                    {
                        int nrOfDigits = GetNumberOfDigits(value);
                        if (nrOfDigits % 2 == 0) // Even number of digits
                        {
                            var (leftSplit, rightSplit) = SplitValue(value, nrOfDigits);
                            stone.Value = rightSplit;
                            stoneList.AddBefore(stone, leftSplit);
                        }
                        else // Multiply by 2024
                        {
                            stone.Value = value * 2024;
                        }
                    }

                    // Prep next iteration
                    stone = stone.Next!;
                }
            }

            TextUtilities.CFWLine($"@Gra >>> There are now @Yel{stoneList.Count} stones.");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // 65601038650482 too low
            // Oops I ran with the simple dataset..
            // Correct is 234430066982597

            // Using a linked list will lead to 2^75 ~10^22 occurances
            // We will need another way to store the values.
            // Because positions don't really matter we might want to use a Dictionary.
            // This way we cna track HOW MANY INSTANCES of the same number occurs.
            Dictionary<long, long> stones = ParseToDict(DataLoader.LoadAllData(11, fullOrSimpleData));

            //LinkedList<long> stoneList = Parse(DataLoader.LoadAllData(11, fullOrSimpleData));

            TextUtilities.CFWLine("@GraThe first @Yel6 @Grablinks:");
            for (int i = 0; i < 75; i++)
            {
                // Draw
                if (i < 7)
                {
                    foreach (var item in stones)
                    {
                        TextUtilities.CFW($"@Yel{item.Key.ToString()}@DGy, {item.Value.ToString()} @Gra| ");
                    }
                    Console.WriteLine('\n' + new string('-', Console.WindowWidth - 1));
                }

                // Perform blink for each stone
                List<long> stoneKeys = new List<long>(stones.Keys);
                List<long> stoneValues = new List<long>(stones.Values);
                for (int stoneID = 0; stoneID < stoneKeys.Count; stoneID++)
                {
                    long value = stoneKeys[stoneID];
                    long amount = stoneValues[stoneID];

                    // Act
                    if (value == 0) // Become 1
                    {
                        AddToStones(1, amount);
                    }
                    else
                    {
                        int nrOfDigits = GetNumberOfDigits(value);
                        if (nrOfDigits % 2 == 0) // Even number of digits
                        {
                            var (leftSplit, rightSplit) = SplitValue(value, nrOfDigits);

                            // Add right split
                            AddToStones(rightSplit, amount);

                            // Add left split
                            AddToStones(leftSplit, amount);
                        }
                        else // Multiply by 2024
                        {
                            AddToStones(value * 2024, amount);
                        }
                    }

                    // Subtract from amount
                    if (amount > stones[value]) throw new Exception("Bruh we got an amount that was bigger than the amount of stones..");
                    stones[value] -= amount;
                    if(stones[value] <= 0)
                        stones.Remove(value);
                }
            }

            long result = 0;
            foreach (var stone in stones)
            {
                result += stone.Value;
            }
            TextUtilities.CFWLine($"@Gra >>> There are now @Yel{stones.Sum(x => x.Value)} stones.");

            void AddToStones(long value, long times)
            {
                if (stones.ContainsKey(value))
                    stones[value] += times;
                else stones.Add(value, times);
            }
        }

        public (long leftSplit, long rightSplit) SplitValue(long value)
        {
            return SplitValue(value, GetNumberOfDigits(value));
        }
        public (long leftSplit, long rightSplit) SplitValue(long value, int nrOfDigits)
        {
            // Some math that splits a value
            long leftSplit, rightSplit;
            long powerOfTen = (long)Math.Pow(10, nrOfDigits / 2);

            leftSplit = value/powerOfTen;
            rightSplit = value - powerOfTen * leftSplit;
            return (leftSplit, rightSplit);
        }

        public (int leftSplit, int rightSplit) SplitValueInt(int value)
        {
            return SplitValueInt(value, GetNumberOfDigits(value));
        }
        public (int leftSplit, int rightSplit) SplitValueInt(int value, int nrOfDigits)
        {
            // Some math that splits a value
            int leftSplit, rightSplit;
            int powerOfTen = (int)Math.Pow(10, nrOfDigits / 2);

            leftSplit = value / powerOfTen;
            rightSplit = value - powerOfTen * leftSplit;
            return (leftSplit, rightSplit);
        }

        int GetNumberOfDigits(long value) => (int)(Math.Log10(value) + 1);

        LinkedList<long> Parse(string data) => new LinkedList<long>(from nr in data.Split(' ') select long.Parse(nr));
        Dictionary<long, long> ParseToDict(string data)
        {
            Dictionary<long, long> dict = new Dictionary<long, long>();
            foreach (var split in data.Split(' '))
            {
                long parsedLong = long.Parse(split);
                if (dict.ContainsKey(parsedLong))
                    parsedLong++;
                else dict.Add(parsedLong, 1);
            }
            return dict;
        }
    }
}
