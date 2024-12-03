using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.Linq;

namespace AdventOfCode2024.Days
{
    class Day2 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // How to access: reports[row][column]
            // One row = reports[i]
            // You can't get a specific column
            int[][] reports = (from row in DataLoader.LoadRowData(2, fullOrSimpleData)
                                  select (from nr in row.Split(' ') select int.Parse(nr)).ToArray())
                                  .ToArray();

            int numberOfRows = reports.Length;

            int numberOfSafeReports = 0;

            // For each report (row)
            for (int i = 0; i < reports.Length; i++)
            {
                int[] report = reports[i];
                string reportAsString = string.Join(' ', report);

                bool gradual = !DiffersByMoreThan(3, report);
                bool increasing = AreAllDecreasing(report);
                bool decreasing = AreAllIncreasing(report);

                Console.ForegroundColor = ConsoleColor.Yellow;
                int digits = (int)Math.Log10(numberOfRows) + 1; // calculate how many digits we want to display for the index
                Console.Write(i.ToString($"d{digits}") + ":(" + reportAsString + ") ");

                // Determine safe or unsafe
                if (gradual && (increasing || decreasing))
                {
                    numberOfSafeReports++;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Safe!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Unsafe. ");
                    if (!gradual) Console.Write("Not gradual. ");
                    if (!increasing && ! decreasing) Console.Write("Not linear.");
                    Console.WriteLine();
                }
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nNumber of safe reports: " + numberOfSafeReports);
            Console.ResetColor();
        }

        bool DiffersByMoreThan(int maxLength, int[] input)
        {
            for (int i = 1; i < input.Length; i++)
            {
                if (Math.Abs(input[i] - input[i - 1]) > maxLength) return true;
            }
            return false;
        }

        bool AreAllIncreasing(int[] input)
        {
            for (int i = 1; i < input.Length; i++)
                if (input[i] <= input[i - 1]) return false;
            return true;
        }
        bool AreAllDecreasing(int[] input)
        {
            for (int i = 1; i < input.Length; i++)
                if (input[i] >= input[i - 1]) return false;
            return true;
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // How to access: reports[row][column]
            // One row = reports[i]
            // You can't get a specific column
            int[][] reports = (from row in DataLoader.LoadRowData(2, fullOrSimpleData)
                               select (from nr in row.Split(' ') select int.Parse(nr)).ToArray())
                                  .ToArray();

            int numberOfRows = reports.Length;

            int numberOfSafeReports = 0;

            // For each report (row)
            // We se if the report satisfies our condition.
            // If it doesn't, we try to change it by removing one "level" at a time.
            // If any of them work we see it as a valid report.
            for (int i = 0; i < reports.Length; i++)
            {
                int[] report = reports[i];
                string reportAsString = string.Join(' ', report);

                Console.ForegroundColor = ConsoleColor.Yellow;
                int digits = (int)Math.Log10(numberOfRows) + 1; // calculate how many digits we want to display for the index
                Console.Write(i.ToString($"d{digits}") + ":(" + reportAsString + ") ");

                // Does the report satisfy our condition?
                if (Condition(report)) // If gradual AND (increasing OR decreasing)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Safe as is!");
                    numberOfSafeReports++;
                }
                else // If report doesn't satisfy our condition we change it
                {
                    bool success = false; // To be able to write an output we need this to flag if the below for loop was successful
                    
                    // Systematically remove one element and check if that is safe
                    for (int j = 0; j < report.Length; j++) // j counts what element to skip
                    {
                        // Create new report. Do not include element j
                        int[] newReport = new int[report.Length - 1];
                        for (int k = 0; k < newReport.Length; k++) // k counts the element in list
                        {
                            if (k >= j)
                                newReport[k] = report[k+1];
                            else
                                newReport[k] = report[k];
                        }

                        // If the new report works
                        if (Condition(newReport))
                        {
                            string newReportAsString = string.Join(' ', newReport);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Safe as (" + newReportAsString + ")");
                            success = true;
                            numberOfSafeReports++;
                            break;
                        }
                    }

                    // If none of the edited reports work, the original report is always unsafe
                    if (!success)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Always unsafe!");
                    }
                }
                Console.ResetColor();
            }

            // Output
            Console.Write("\nNumber of safe reports: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(numberOfSafeReports);
            Console.ResetColor();
        }

        // A simple function for checking the validity of a report
        bool Condition(int[] report) => !DiffersByMoreThan(3, report) && (AreAllDecreasing(report) || AreAllIncreasing(report));
    }
}
