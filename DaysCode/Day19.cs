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
            // My first idea was to do the same as part one, but keep a record of what has been done to split.
            // This proved kind of difficult. I only managed to find some solutions.
            // My method:
            // I keep a history of all parts that were ever removed, starting with only small parts.
            // This ensures I get the most parts possible. Then I sort them in size order (longer patterns first)
            // and reconstruct the original design, but divided into the parts
            // brrrwggg => (unpredictible order) r rwg gg br => rwg gg br r => br r rwg gg
            // With this I can start to try and combine the small parts into bigger parts. If it succeeds, we have found
            // another way to build the design
            // "br r rwg gg" could become "brr rwg gg", or "br rrwg gg", or "br r rwggg"
            // I decided to not continue on this, but it should work in theory if you keep doing this recursively.

            // Instead I looked at a user "zniperr"'s solution
            // https://www.reddit.com/r/adventofcode/comments/1hhlb8g/comment/m3f681k/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
            // I initially did a splitting method. Zniperr strips away patterns one by one from the front and then
            // keeps recursively doing the same on the next. Much more elegant. If a part ever becomes empty, it
            // returns 1. This represents 1 way to split it all up. Because we do this recursively, we collect all
            // the ones.

            // 1041529704688380 correct!

            string[] data = DataLoader.LoadRowData(19, fullOrSimpleData);
            string[] designs = data[2..];
            string[] towels = data[0].Split(", ");

            // Sorting by largest first makes the algorithm faster
            towels = towels.OrderBy(towel => -towel.Length).ToArray();

            long amountSolvable = 0;
            foreach (var design in designs)
            {
                long result = GetTimesSolvable(design, towels);
                if (result == 0)
                {
                    TextUtilities.CFWLine("@DReUnsolvable: @Yel" + design + "@Gra");
                }
                else
                {
                    TextUtilities.CFWLine("@GreSolvable:   @Yel" + design.PadRight(65) + "@GraVariations: @Yel" + result);
                    amountSolvable += result;
                }
            }

            TextUtilities.CFWLine("@Gra >>> Number of solvable: @Yel" + amountSolvable);
        }

        private readonly Dictionary<string, bool> memoP1 = new Dictionary<string, bool>();
        private readonly Dictionary<string, long> memoP2 = new Dictionary<string, long>();

        long GetTimesSolvable(string design, string[] towelsInput)
        {
            //memoP2.Clear();

            return TimesSolvable(design);

            long TimesSolvable(string design)
            {
                // Memoization to make it super fast
                // This means we remember how many solutions a
                // particular design has. A lookup table.
                if (memoP2.ContainsKey(design))
                    return memoP2[design];

                long result = 0;

                // If the design is empty it means we reached the end of a solvable design.
                // We return 1.
                if (design == "") return 1; 

                for (int i = 0; i < towelsInput.Length; i++)
                {
                    string pattern = towelsInput[i];
                    if (design.StartsWith(pattern))
                    {
                        // If the design starts with a pattern, we remove the
                        // pattern from the start of the design.
                        string nextDesign = design[pattern.Length..];

                        // If the next design contains a solvable way we
                        // add it to the result. When all results are
                        // combined we will have all solvable ways.
                        result += TimesSolvable(nextDesign);
                    }
                }

                // Memoization to make it super fast
                if (!memoP2.ContainsKey(design))
                    memoP2.Add(design, result);

                return result;
            }
        }

        bool IsSolvable(string design, string[] towelsInput)
        {
            // Clear memoization memory
            memoP1.Clear();

            bool writeOutput = false;
            long activeTrySplit = 0;

            return TrySplit(design);

            bool TrySplit(string d)
            {
                if (memoP1.ContainsKey(d))
                    return memoP1[d];

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

                        memoP1.Add(d, true);
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
                            memoP1.Add(d, result);
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
                            memoP1.Add(d, result);
                            return result;
                        }
                    }
                }
                memoP1.Add(d, result);
                return result;
            }
        }
    }
}
