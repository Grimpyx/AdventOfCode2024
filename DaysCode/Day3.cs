using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.Linq;

namespace AdventOfCode2024.Days
{
    class Day3 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            string[] allInstructions = DataLoader.LoadRowData(3, fullOrSimpleData);

            int total = 0;

            foreach (string row in allInstructions)
            {
                for (int i = 0; i < row.Length-4; i++)
                {
                    // If the start of an instruction is found
                    if (row[i..(i+4)] == "mul(")
                    {
                        bool abort = false;

                        int bumper = 4;

                        List<int> firstDigits = new List<int>();
                        List<int> secondDigits = new List<int>();

                        // Look for numbers
                        while (true)
                        {
                            if (char.IsDigit(row[i + bumper])) // Keep counting if numbers
                            {
                                firstDigits.Add((int)char.GetNumericValue(row[i + bumper]));
                                bumper++;
                            }
                            else if (row[i + bumper] != ',') // not a number, and illegal character
                            {
                                abort = true;
                                break;
                            }
                            else break; // not a number, but legal character (for example if it's ','
                        }
                        if (abort) continue;

                        // if the char after the number is not a ','
                        if (row[i + bumper] != ',') continue;
                        bumper++;

                        // Look for numbers again
                        while (true)
                        {
                            if (char.IsDigit(row[i + bumper])) // Keep counting if numbers
                            {
                                secondDigits.Add((int)char.GetNumericValue(row[i + bumper]));
                                bumper++;
                            }
                            else if (row[i + bumper] != ')') // not a number, and illegal character
                            {
                                abort = true;
                                break;
                            }
                            else break; // not a number, but legal character (for example if it's ')'
                        }
                        if (abort) continue;

                        // If not aborted by now, everything should be valid.
                        int completeNumber1 = int.Parse(String.Concat(firstDigits));
                        int completeNumber2 = int.Parse(String.Concat(secondDigits));
                        total += completeNumber1 * completeNumber2;
                        Console.WriteLine($"Multiplying {completeNumber1} with {completeNumber2}. Adding {completeNumber1 * completeNumber2} to total. New total is {total}.");
                    }
                }
            }

            Console.ResetColor();
            Console.Write("Total value is: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(total);
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            string[] allInstructions = DataLoader.LoadRowData(3, fullOrSimpleData);

            int total = 0;
            int nrOfSkippedInstructions = 0;

            bool dontIsActive = false;
            Console.WriteLine("Complete code:\n  " + allInstructions[0] + "\n");

            foreach (string row in allInstructions)
            {
                for (int i = 0; i < row.Length - 4; i++)
                {
                    if (!dontIsActive && i + 7 < row.Length && row[i..(i+7)] == "don't()")
                    {
                        TextUtilities.ColorWriteLine(ConsoleColor.Yellow, " Found 'don't()' @ i=" + i);
                        TextUtilities.ColorWriteLine(ConsoleColor.Red, " Skipping until 'do()' is found...");
                        dontIsActive = true;
                    }
                    else if (dontIsActive && i + 4 < row.Length && row[i..(i+4)] == "do()")
                    {
                        TextUtilities.ColorWriteLine(ConsoleColor.Red, $" Skipped a total of {nrOfSkippedInstructions} instructions");
                        nrOfSkippedInstructions = 0;

                        TextUtilities.ColorWriteLine(ConsoleColor.Yellow, " Found 'do()'    @ i=" + i);
                        TextUtilities.ColorWriteLine(ConsoleColor.Green, " Resuming instructions...");
                        dontIsActive = false;
                    }

                    if (dontIsActive)
                    {
                        nrOfSkippedInstructions++;
                        continue;
                    }

                    // If the start of an instruction is found
                    if (row[i..(i + 4)] == "mul(")
                    {
                        bool abort = false;

                        int bumper = 4;

                        List<int> firstDigits = new List<int>();
                        List<int> secondDigits = new List<int>();

                        // Look for numbers
                        while (true)
                        {
                            if (char.IsDigit(row[i + bumper])) // Keep counting if numbers
                            {
                                firstDigits.Add((int)char.GetNumericValue(row[i + bumper]));
                                bumper++;
                            }
                            else if (row[i + bumper] != ',') // not a number, and illegal character
                            {
                                abort = true;
                                break;
                            }
                            else break; // not a number, but legal character (for example if it's ','
                        }
                        if (abort) continue;

                        // if the char after the number is not a ','
                        if (row[i + bumper] != ',') continue;
                        bumper++;

                        // Look for numbers again
                        while (true)
                        {
                            if (char.IsDigit(row[i + bumper])) // Keep counting if numbers
                            {
                                secondDigits.Add((int)char.GetNumericValue(row[i + bumper]));
                                bumper++;
                            }
                            else if (row[i + bumper] != ')') // not a number, and illegal character
                            {
                                abort = true;
                                break;
                            }
                            else break; // not a number, but legal character (for example if it's ')'
                        }
                        if (abort) continue;

                        // If not aborted by now, everything should be valid.
                        int completeNumber1 = int.Parse(String.Concat(firstDigits));
                        int completeNumber2 = int.Parse(String.Concat(secondDigits));
                        total += completeNumber1 * completeNumber2;
                        Console.WriteLine($" : Multiplying {completeNumber1} with {completeNumber2}. Adding {completeNumber1 * completeNumber2} to total. New total is {total}.");
                    }
                }
            }

            Console.ResetColor();
            Console.Write(" >>> Total value is: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(total);
        }
    }
}
