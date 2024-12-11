using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

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
                if (i < 8)
                {
                    foreach (var item in stoneList)
                    {
                        Console.Write(item.ToString() + ' ');
                    }
                    Console.WriteLine();
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

            Console.WriteLine();
            TextUtilities.CFWLine($"@Gra >>> There are now @Yel{stoneList.Count} stones.");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            LinkedList<long> stoneList = Parse(DataLoader.LoadAllData(11, fullOrSimpleData));

            TextUtilities.CFWLine("@GraThe first @Yel6 @Grablinks:");
            for (int i = 0; i < 25; i++)
            {
                // Draw
                if (i < 8)
                {
                    foreach (var item in stoneList)
                    {
                        Console.Write(item.ToString() + ' ');
                    }
                    Console.WriteLine();
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

            Console.WriteLine();
            TextUtilities.CFWLine($"@Gra >>> There are now @Yel{stoneList.Count} stones.");
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

        int GetNumberOfDigits(long value) => (int)(Math.Log10(value) + 1);

        LinkedList<long> Parse(string data) => new LinkedList<long>(from nr in data.Split(' ') select long.Parse(nr));
    }
}
