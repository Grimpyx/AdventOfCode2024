using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day1 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            long totalDifference = 0;

            string[] data = DataLoader.LoadRowData(1, fullOrSimpleData);
            int[] col1 = new int[data.Length];
            int[] col2 = new int[data.Length];

            // Make an array of numbers for each column
            for (int i = 0; i < data.Length; i++)
            {
                string[] row_i_column = data[i].Split("   ");
                col1[i] = int.Parse(row_i_column[0]);
                col2[i] = int.Parse(row_i_column[1]);
            }

            // Sort numbers from smallest to biggest
            Array.Sort(col1);
            Array.Sort(col2);

            // Sum all differences
            for (int i = 0; i < data.Length; i++)
            {
                totalDifference += Math.Abs(col1[i] - col2[i]);
                //Console.WriteLine("Adding difference between {0} and {1}. Current is {2}.", col1[i], col2[i], totalDifference);
            }
            Console.WriteLine("Total difference is {0}", totalDifference);
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            long similarityScore = 0;

            string[] data = DataLoader.LoadRowData(1, fullOrSimpleData);
            int[] col1 = new int[data.Length];
            int[] col2 = new int[data.Length];

            // Make an array of numbers for each column
            for (int i = 0; i < data.Length; i++)
            {
                string[] row_i_column = data[i].Split("   ");
                col1[i] = int.Parse(row_i_column[0]);
                col2[i] = int.Parse(row_i_column[1]);
            }

            // Sort numbers from smallest to biggest
            Array.Sort(col1);
            Array.Sort(col2);

            // Calculate similarity
            for (int i = 0; i < data.Length; i++)
            {
                int currentComparer = col1[i];

                if (currentComparer < col2[0] || currentComparer > col2[^1]) // if it doesn't exist within the bounds
                    continue;
                Console.WriteLine();
                // Since the columns are sorted, all are in order
                int lengthStep = data.Length / 2;
                int currentFirstIndex = lengthStep;
                for (int j = 0; j < 30; j++)
                {
                    lengthStep /= 2; // Decrease step with each iteration
                    if (lengthStep <= 0) lengthStep = 1;

                    if (col2[currentFirstIndex] > currentComparer)
                        currentFirstIndex -= lengthStep;
                    else if (col2[currentFirstIndex] < currentComparer)
                        currentFirstIndex += lengthStep;
                    else break;

                    //Console.WriteLine($"Searching for {currentComparer}. Jump {j}: {col2[currentFirstIndex]}@{currentFirstIndex}");
                }

                Console.Write(currentComparer + ": ");

                if (currentComparer == col2[currentFirstIndex])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("MATCH!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("NO MATCH! CONTINUING");
                    Console.ResetColor();
                    continue;
                }

                // We found at least one match at index currentFirstIndex.
                // We now go backward until we hit a NON match.
                // Afterwards we walk forward and count the amount of matches.
                while (currentComparer == col2[currentFirstIndex])
                {
                    currentFirstIndex--;
                    if (currentFirstIndex < 0) break;
                }

                // Start from a valid index
                currentFirstIndex++;
                int counter = 0;
                while (currentComparer == col2[currentFirstIndex])
                {
                    counter++;
                    currentFirstIndex++;
                }
                Console.WriteLine($"Found a total of {counter} occurences.");
                similarityScore += currentComparer * counter;
                Console.WriteLine($"Adding ({currentComparer} * {counter} = {currentComparer * counter}) to similarity score. New total: {similarityScore}");



                /*int howManyTimesItAppears = 0;
                for (int j = 0; j < data.Length; j++)
                {

                }*/
                //similarityScore += Math.Abs(col1[i] - col2[i]);
                //Console.WriteLine("Adding difference between {0} and {1}. Current is {2}.", col1[i], col2[i], similarityScore);
            }
            Console.Write($"\nTotal similarity score: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(similarityScore);
            Console.ResetColor();
        }

    }
}
