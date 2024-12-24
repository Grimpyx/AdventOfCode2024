using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.Linq;

namespace AdventOfCode2024.Days
{
    class Day19 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // My first solution was an attempt at making a recursive queue.
            // It worked for the simple dataset but not the full, everything was apparently solvable.
            // Instead I set for a recursive solution:
            // If the design is "splitable" with anything in the towel pattern list,
            // and if the result of the split is also splitable, on and on and on, then the whole is splitable.
            // It worked well for the simple dataset, and the two first in the full. The rest just took way too long.
            // So I implemented memoization. What it does is it remembers the result of the recursive function for different inputs.
            // If the function has already run with a specific input, the output will be the same, so we have a lookup table.

            string[] data = DataLoader.LoadRowData(19, fullOrSimpleData);
            string[] designs = data[2..];
            string[] towels = data[0].Split(", ");

            // The order of the towels impact the speed.
            // The following orders in increasing speed.
            // Largest patterns first resulted in ~1500ms
            // Small patterns first resulted in ~265ms
            // No sorting at all results in ~410ms
            towels = towels.OrderBy(towel => towel.Length).ToArray(); // Sort by size. Small comes first.
            //towels = towels.OrderBy(towel => -towel.Length).ToArray(); // Sort by size. Large comes first.

            int amountSolvable = 0;
            foreach (var design in designs)
            {
                //var design = "wgrggwgruubbgbgurwrbgggwbuwwruburwrbrwwgrubg";
                bool solvable = IsSolvable(design, towels);

                if (!solvable)
                {
                    TextUtilities.CFWLine("@DReUnsolvable: @Yel" + design + "@Gra");
                }
                else
                {
                    TextUtilities.CFWLine("@GreSolvable:   @Yel" + design + "@Gra");
                    amountSolvable++;
                }
            }

            TextUtilities.CFWLine("@Gra >>> Number of solvable: @Yel" + amountSolvable);
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
        }

        private static Dictionary<string, bool> memo = new Dictionary<string, bool>();

        bool IsSolvable(string design, string[] towelsInput)
        {
            // Clear memoization memory
            memo.Clear();

            bool writeOutput = false;
            long activeTrySplit = 0;

            return TrySplit(design);

            bool TrySplit(string d)
            {
                if (memo.ContainsKey(d))
                    return memo[d];

                activeTrySplit++;

                bool result = false;
                for (int i = 0; i < towelsInput.Length; i++)
                {
                    var split = d.Split(towelsInput[i], StringSplitOptions.RemoveEmptyEntries);

                    // we managed to split away everything
                    if (split.Length == 0)
                    {
                        if (writeOutput)
                        {
                            Console.WriteLine("Found | " + towelsInput[i]);
                            Console.WriteLine("Rest  | NONE");
                        }

                        memo.Add(d, true);
                        return true;
                    }

                    // with only one split element
                    else if (split.Length == 1)
                    {
                        // if that split rest is the same as input string
                        // we couldn't split it
                        if (split[0] == d)
                        {
                            continue;
                        }

                        /////// Queue rest
                        if (writeOutput)
                        {
                            Console.WriteLine("Found | " + towelsInput[i]);
                            Console.WriteLine("Rest  | " + split[0]);
                        }
                        result |= TrySplit(split[0]);
                        if (result)
                        {
                            memo.Add(d, result);
                            return result;
                        }
                    }

                    // If we managed to split and we get at least two parts
                    else
                    {
                        if (writeOutput)
                        {
                            Console.WriteLine("Found | " + towelsInput[i]);
                            Console.WriteLine("Rest  | " + string.Join('|', split));
                        }
                        bool temp = true;
                        for (int j = 0; j < split.Length; j++)
                        {
                            temp &= TrySplit(split[j]);
                        }
                        result |= temp;
                        if (result)
                        {
                            memo.Add(d, result);
                            return result;
                        }
                    }
                }
                //if (!memo.ContainsKey(d))
                    memo.Add(d, result);
                return result;
            }

            /*LinkedList<string> list = new LinkedList<string>(towelsInput);

            
            //bool isSolvable = false;
            for (int startTowelIndex = 0; startTowelIndex < towelsInput.Length; startTowelIndex++)
            {
                TextUtilities.CFWLine($"@GraStarting @Cya{design} @Grafrom index @Cya{startTowelIndex}@Gra");
                if (startTowelIndex != 0)
                {
                    string firstNode = list.First!.ValueRef;
                    list.RemoveFirst();
                    list.AddLast(firstNode);
                }
                string[] towels = list.ToArray();


                Queue<string> queue = new Queue<string>([design]);
                while (queue.Count > 0)
                {
                    var s = queue.Dequeue();


                    for (int towelIndex = 0; towelIndex < towels.Length; towelIndex++)
                    {
                        string towel = towels[towelIndex];

                        var divided_s = s.Split(towel, StringSplitOptions.RemoveEmptyEntries);

                        // If the (towel == s)
                        // Continue without queuing
                        if (divided_s.Length == 0)
                        {
                            //Console.ForegroundColor = ConsoleColor.Red;
                            //Console.WriteLine(new string('-', 50));
                            TextUtilities.CFWLine("@GraSplitting @Yel" + s + "@Gra with @Yel" + towel + "@Gra. Rest: @Mgn" + string.Join("@Gra|@Mgn", divided_s));
                            TextUtilities.CFWLine("@Yel" + s + " @GraExists. Removing.");
                            if (queue.Count == 0)
                            {
                                return true;
                            }
                            break;
                        }
                        // If rest.length is 1 and its remains is the same as the split, it means we couldn't split
                        // Just continue to see if something else can split it
                        else if (divided_s.Length == 1 && divided_s[0] == towel)
                        {
                            continue;
                        }
                        else if (divided_s.Length == 1 && divided_s[0] == s)
                        {
                            continue;
                        }
                        else
                        {
                            // If we managed to split, queue next and go to next in queue
                            for (int i = 0; i < divided_s.Length; i++)
                            {
                                queue.Enqueue(divided_s[i]);
                            }

                            //Console.ForegroundColor = ConsoleColor.Red;
                            //Console.WriteLine(new string('-', 50));
                            TextUtilities.CFWLine("@GraSplitting @Yel" + s + "@Gra with @Yel" + towel + "@Gra. Rest: @Mgn" + string.Join("@Gra|@Mgn", divided_s));
                            break;
                        }



                    }
                }
            }
            return false;*/
        }
    }
}
