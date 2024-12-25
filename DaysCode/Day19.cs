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
            // This proved kind of difficult. I only managed to find some solutions. My method:
            // I keep a history of all parts that were ever removed, starting with only small parts.
            // This ensures I get the most parts possible. Then I sort them in size order (longer patterns first)
            // and reconstruct the original design, but with the parts
            // brrwggg => b r gg rwg => rwg gg b r
            // With this I can start to try and combine the small parts into bigger parts. If it succeeds, we have found
            // another way to build the design, rwg gg b r => rwg gg br
            // I decided to not continue on this, but it should work in theory if you keep doing this recursively.

            // Instead I looked at a user "zniperr"'s solution
            // https://www.reddit.com/r/adventofcode/comments/1hhlb8g/comment/m3f681k/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
            // I initially did a splitting method. Parts one by one from the front, much more elegant.

            // 1041529704688380

            string[] data = DataLoader.LoadRowData(19, fullOrSimpleData);
            string[] designs = data[2..];
            string[] towels = data[0].Split(", ");

            // Sorting by largest first makes the algorithm faster
            towels = towels.OrderBy(towel => -towel.Length).ToArray();

            long amountSolvable = 0;
            foreach (var design in designs)
            {
                long res = GetTimesSolvable(design, towels);
                if (res == 0)
                {
                    //TextUtilities.CFWLine("@DReUnsolvable: @Yel" + design + "@Gra");
                }
                else
                {
                    //TextUtilities.CFWLine("@GreSolvable:   @Yel" + design.PadRight(70) + "@GraVariations: @Yel" + res);
                    amountSolvable += res;
                }

                // My previous solution
                /*(bool solvable, List<string> parts) = IsSolvableP2(design, towels);

                TextUtilities.CFWLine("@Mgn" + new string('-', Console.WindowWidth) + "@Gra");
                if (!solvable)
                {
                    TextUtilities.CFWLine("@DReUnsolvable: @Yel" + design + "@Gra");
                }
                else
                {
                    TextUtilities.CFWLine("@Gre" + design + "@Gra");
                    //int add = GetUniqueVariations(design, parts);
                    TextUtilities.CFWLine("@GreSolvable:   @Yel" + design.PadRight(70) + "@GraVariations: @Yel" + add);
                    amountSolvable += add;
                }*/
            }

            TextUtilities.CFWLine("@Gra >>> Number of solvable: @Yel" + amountSolvable);

            int GetUniqueVariations(string goalDesign, List<string> parts)
            {
                // Since the list of parts only says what appears at least once, in no particular order,
                // we have to sort it and rebuild the goal design with its smallest parts
                parts = parts.OrderBy(x => -x.Length).ToList();

                // Moving wave that gets larger
                int unique = 1;
                List<(string part, int index)> partIndexList = new List<(string part, int index)>();
                HashSet<int> indexesToSkip = new HashSet<int>();
                foreach (var part in parts)
                {
                    for (int charIndex = 0; charIndex < goalDesign.Length - part.Length + 1; charIndex++)
                    {
                        if (indexesToSkip.Contains(charIndex)) continue;

                        string s = goalDesign[charIndex..(charIndex + part.Length)];
                        if (s == part)
                        {
                            partIndexList.Add((s, charIndex));
                            for (int i = 0; i < part.Length; i++)
                            {
                                indexesToSkip.Add(charIndex + i);
                            }
                        }
                    }
                }
                Console.WriteLine("Parts:            " + string.Join(',', parts));
                Console.WriteLine("Parts with index: " + string.Join(',', partIndexList));
                parts = partIndexList.OrderBy(x => x.index).Select(x => x.part).ToList();
                Console.WriteLine("Order by index:   " + string.Join(',', parts));

                // 0123456
                // ---VVV   (width 2, i 3)
                // ubwgrbb  (does grb exist in towel pattern list?)
                for (int width = 1; width < parts.Count; width++) // Grow scan area
                {
                    for (int i = 0; i < parts.Count - width; i++)
                    {
                        string join = string.Join("", parts[i..(i + width+1)]);

                        // If it exists in the list of available patterns add one to unique
                        for (int j = 0; j < towels.Length; j++)
                        {
                            if (towels[j].Length == join.Length)
                            {
                                if (towels[j] == join)
                                {
                                    TextUtilities.CFWLine($"@Gre{string.Join(',', parts[i..(i + width+1)])}@Gra => @Gre{join}");
                                    unique++;
                                }
                            }
                        }
                    }
                }
                return unique;
            }
        }

        private static Dictionary<string, bool> memoP1 = new Dictionary<string, bool>();
        private static Dictionary<string, (bool result, List<string> parts)> memoP2 = new Dictionary<string, (bool result, List<string> parts)>();

        private static Dictionary<string, long> memoP22 = new Dictionary<string, long>();

        long GetTimesSolvable(string design, string[] towelsInput)
        {
            //memoP22.Clear();

            return TimesSolvable(design);

            long TimesSolvable(string design)
            {
                if (memoP22.ContainsKey(design))
                    return memoP22[design];

                long result = 0;

                if (design == null || design == "") return 1;

                for (int i = 0; i < towelsInput.Length; i++)
                {
                    string pattern = towelsInput[i];
                    if (design.StartsWith(pattern))
                    {
                        string nextDesign = design[pattern.Length..];
                        result += TimesSolvable(nextDesign);
                    }
                }

                if (!memoP22.ContainsKey(design)) memoP22.Add(design, result);
                return result;
            }
        }


        (bool result, List<string> parts) IsSolvableP2(string design, string[] towelsInput)
        {
            // Clear memoization memory
            memoP2.Clear();

            //List<string> parts = new List<string>();

            bool writeOutput = false;

            return TrySplit(design);

            (bool success, List<string> history) TrySplit(string d)
            {
                //if (memoP2.ContainsKey(d))
                //    return memoP2[d];

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

                        //memoP2.Add(d, (true, [history + d]));
                        return (true, [towelsInput[i]]);
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
                        var r = TrySplit(split[0]); // , [towelsInput[i]]
                        result |= r.success;
                        if (result)
                        {
                            //memoP2.Add(d, (true, [history + towelsInput[i]]));
                            return (true, [.. r.history, towelsInput[i]]);
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
                        List<string> templist = new List<string>();
                        for (int j = 0; j < split.Length; j++)
                        {
                            var spp = TrySplit(split[j]); //, [history + towelsInput[i]]
                            templist.AddRange(spp.history);
                            temp &= spp.success;
                        }
                        result |= temp;
                        if (result)
                        {
                            //memoP2.Add(d, (true, [history + towelsInput[i]]));
                            return (true, [.. templist, towelsInput[i]]);
                            //memoP2.Add(d, (true, [.. history, .. r.history]));
                            //return (true, [.. history, .. r.history]);
                        }
                    }
                }
                // Clear list of history if we ever return false
                //memoP2.Add(d, (false, history));
                return (false, []);
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
