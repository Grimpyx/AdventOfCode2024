using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day22 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            int[] rowData = DataLoader.LoadRowData(22, fullOrSimpleData).Select(int.Parse).ToArray();

            long sum = 0;
            for (int numberIndex = 0; numberIndex < rowData.Length; numberIndex++)
            {
                long value = rowData[numberIndex];
                TextUtilities.CFWLine($"@Red{value}@Gra:");

                for (int i = 0; i < 2000; i++)
                {
                    value = NextNumber(value);
                    if (i == 1999) TextUtilities.CFWLine($"@Red {i + 1:D2}@Gra: @Yel{value}");
                }
                sum += value;
            }
            Console.WriteLine();
            TextUtilities.CFWLine($"@Gra >>> Sum of all secret numbers after 2000 repetitions: @Yel{sum}");


            long NextNumber(long secretNumber)
            {
                Mix(secretNumber << 6);  // mult 64 (2^6)
                Prune();

                Mix(secretNumber >> 5);  // div 32 (2^5)
                Prune();

                Mix(secretNumber << 11); // mult 2024 (2^11)
                Prune();

                return secretNumber;

                void Mix(long value) => secretNumber ^= value;

                void Prune()
                {
                    secretNumber &= 0b1111_1111_1111_1111_1111_1111; // Represents modulo 1677216 (which is 2^24)
                    //secretNumber %= 16777216;
                }
            }
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            int[] rowData = DataLoader.LoadRowData(22, fullOrSimpleData).Select(int.Parse).ToArray();

            // Create all number sequences (modulus 10 (% 10) gives the price)
            int[][] sequences = new int[rowData.Length][];
            for (int numberIndex = 0; numberIndex < rowData.Length; numberIndex++)
            {
                int value = rowData[numberIndex];

                int[] sequence = new int[2000];
                sequence[0] = value;
                for (int i = 1; i < sequence.Length; i++)
                {
                    value = NextNumber(value);
                    sequence[i] = value;
                }
                sequences[numberIndex] = sequence;
            }

            // Create the sequence of differences
            int[][] diffSequences = new int[rowData.Length][];
            for (int numberIndex = 0; numberIndex < rowData.Length; numberIndex++)
            {
                int[] seq = new int[sequences[numberIndex].Length];
                seq[0] = int.MaxValue;
                for (int i = 1; i < seq.Length - 1; i++)
                {
                    seq[i] = (int)((sequences[numberIndex][i] % 10) - (sequences[numberIndex][i - 1] % 10));
                }

                diffSequences[numberIndex] = seq;
            }

            // Print
            // Only does it if we have a small dataset
            if (fullOrSimpleData == DayDataType.Simple)
            {
                for (int seqId = 0; seqId < diffSequences.Length; seqId++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (i == 0)
                            TextUtilities.CFWLine($" @DRe{i,5:D4}{"",4} @DRe| @Red" + sequences[seqId][i] + "@Gra");
                        else
                            TextUtilities.CFWLine($" @DRe{i,5:D4}@Yel{diffSequences[seqId][i],4:D2} @DRe| @Yel" + sequences[seqId][i]);
                    }
                    Console.WriteLine("---------------------------------------------------");
                }
            }

            // Smart solution using a dictionary
            // Took 703 ms,
            // The key represents the consecutive diff values. The value is the sum of all buyer's prices for that key.
            Dictionary<(int, int, int, int), int> priceMap = new Dictionary<(int, int, int, int), int>();
            HashSet<(int, int, int, int)> alreadyVisited = new HashSet<(int, int, int, int)>();
            for (int buyerIndex = 0; buyerIndex < sequences.Length; buyerIndex++)
            {
                alreadyVisited.Clear();

                // The lists associated with the current buyer
                int[] sequence = sequences[buyerIndex];
                int[] diffSequence = diffSequences[buyerIndex];

                for (int i = 1; i < sequence.Length - 4; i++)
                {
                    // Diff starts counting at sequence index 1 (index 0 has a value but should not be counted)
                    (int, int, int, int) currentConsecutives = (diffSequence[i], diffSequence[i + 1], diffSequence[i + 2], diffSequence[i + 3]);

                    // this check is required because we only want to grab the price THE FIRST TIME the consecutive sequence appears
                    if (alreadyVisited.Add(currentConsecutives))
                    {
                        int priceValue = sequence[i + 3] % 10;
                        priceMap[currentConsecutives] = priceMap.GetValueOrDefault(currentConsecutives) + priceValue;
                    }
                }
            }

            TextUtilities.CFWLine("@Gra >>> Highest price: @Yel" + priceMap.Values.Max());

            // The following is a brute force solution.
            // Gave the correct anwer (2362) but took 2131179 ms (35 minutes)
            /*
            int highestPrice = int.MinValue;
            int[] highestPriceConsecutive = new int[4];

            for (int i0 = -9; i0 <= 9; i0++)
            {
                Console.WriteLine("i0: " + i0);
                for (int i1 = -9; i1 <= 9; i1++)
                {
                    for (int i2 = -9; i2 <= 9; i2++)
                    {
                        for (int i3 = -9; i3 <= 9; i3++)
                        {
                            // Collect all values
                            int highestPriceCandidate = 0;
                            for (int buyerIndex = 0; buyerIndex < sequences.Length; buyerIndex++)
                            {
                                if (!TryGetPriceFromConsecutive(buyerIndex, [i0,i1,i2,i3], out int price))
                                    continue; // Skip buyer if we found none
                                highestPriceCandidate += price;
                            }
                            if (highestPriceCandidate > highestPrice)
                                highestPrice = highestPriceCandidate;
                        }
                    }
                }
            }

            TextUtilities.CFWLine("@Gra >>> Highest price: @Yel" + highestPrice);

            bool TryGetPriceFromConsecutive(int buyerIndex, int[] consecutive, out int price)
            {
                int[] sequence = sequences[buyerIndex];
                int[] diffSequence = diffSequences[buyerIndex];

                // Diff starts counting at sequence index 1 (index 0 has a value but should not be counted)
                for (int i = 1; i < sequence.Length - 4; i++)
                {
                    if (diffSequence[i] == consecutive[0] &&
                        diffSequence[i + 1] == consecutive[1] &&
                        diffSequence[i + 2] == consecutive[2] &&
                        diffSequence[i + 3] == consecutive[3])
                    {
                        price = (int)(sequence[i + 3] % 10);
                        return true;
                    }
                }

                price = 0;
                return false;
            }
            */

            int NextNumber(int secretNumber)
            {
                Mix(secretNumber << 6);  // mult 64 (2^6)
                Prune();

                Mix(secretNumber >> 5);  // div 32 (2^5)
                Prune();

                Mix(secretNumber << 11); // mult 2024 (2^11)
                Prune();

                return secretNumber;

                void Mix(int value) => secretNumber ^= value;

                void Prune()
                {
                    secretNumber &= 0b1111_1111_1111_1111_1111_1111; // Represents modulo 1677216 (which is 2^24)
                }
            }
        }
    }
}
