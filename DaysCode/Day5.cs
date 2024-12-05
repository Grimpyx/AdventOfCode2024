using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventOfCode2024.Days
{
    class Day5 : IDayChallenge
    {
        public record class NumberWithConditions : IComparer<int>
        {
            public HashSet<int> ToTheLeft { get; }
            public HashSet<int> ToTheRight { get; }

            public NumberWithConditions()
            {
                ToTheLeft = new HashSet<int>();
                ToTheRight = new HashSet<int>();
            }

            public int Compare(int x, int y)
            {
                // x to the left of y
                bool y_isInLeft = ToTheLeft.Contains(y);
                bool y_isInRight = ToTheRight.Contains(y);
                
                if (y_isInLeft && y_isInRight) return 0;

                if (y_isInLeft) return -1;
                else if (y_isInRight) return 1;
                return 0;
            }
        }

        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            int middlePagesSum = 0;

            string[] rows = DataLoader.LoadRowData(5, fullOrSimpleData);
            List<Rule> rules = new List<Rule>();
            List<List<int>> updates = new List<List<int>>();


            #region Input interpretation
            {
                int rowLength = rows.Length;
                int i = 0;
                while (true)
                {
                    if (rows[i] == "") break;
                    string[] split = rows[i].Split('|');
                    rules.Add(new Rule(int.Parse(split[0]), int.Parse(split[1])));
                    i++;
                }
                i++;
                while (i < rowLength)
                {
                    string[] split = rows[i].Split(',');
                    updates.Add((from part in split select int.Parse(part)).ToList());
                    i++;
                }
            }
            #endregion


            // Sort the rule data.
            // For all pages we add a reference to all values that belongs to the left and the right.
            // What this means is that we can later for any number in an update determine if any
            // values to the left or right are incorrectly placed.
            Dictionary<int, NumberWithConditions> nrLookup = new Dictionary<int, NumberWithConditions>();
            foreach (var rule in rules)
            {
                NumberWithConditions con;

                if (nrLookup.TryGetValue(rule.FirstPage, out con))
                {
                    con.ToTheRight.Add(rule.SecondPage);
                }
                else
                {
                    con = new NumberWithConditions();
                    con.ToTheRight.Add(rule.SecondPage);
                    nrLookup.Add(rule.FirstPage, con);
                }


                if (nrLookup.TryGetValue(rule.SecondPage, out con))
                {
                    con.ToTheLeft.Add(rule.FirstPage);
                }
                else
                {
                    con = new NumberWithConditions();
                    con.ToTheLeft.Add(rule.FirstPage);
                    nrLookup.Add(rule.SecondPage, con);
                }
            }

            foreach (List<int> update in updates)
            {
                bool updateIsOk = true; // default is true. Used to stop the search through the active 'update' and to claim it valid or not.

                for (int activeElement = 0; activeElement < update.Count; activeElement++)
                {
                    // con contains all values that are required to the left and the right of a value
                    NumberWithConditions con = nrLookup[update[activeElement]];

                    // loops to the left
                    for (int leftIndex = activeElement - 1; leftIndex > -1; leftIndex--)
                    {
                        if (con.ToTheRight.Contains(update[leftIndex])) // if any value to the left belongs on the right
                        {
                            updateIsOk = false;
                            break;
                        }
                    }
                    if (!updateIsOk) break;

                    // loops to the right
                    for (int rightIndex = activeElement + 1; rightIndex < update.Count; rightIndex++)
                    {
                        if (con.ToTheLeft.Contains(update[rightIndex])) // if any value to the right belongs on the right
                        {
                            updateIsOk = false;
                            break;
                        }
                    }
                    if (!updateIsOk) break;
                }

                Console.Write(string.Join(',', update));
                if (updateIsOk)
                {
                    int previous = middlePagesSum;
                    int val = update[update.Count / 2];
                    middlePagesSum += val;
                    TextUtilities.ColorWrite(ConsoleColor.Green, " OK! ");
                    TextUtilities.ColorWrite(ConsoleColor.Yellow, previous.ToString());
                    Console.Write(" + ");
                    TextUtilities.ColorWrite(ConsoleColor.Green, val.ToString());
                    Console.Write(" -> ");
                    TextUtilities.ColorWriteLine(ConsoleColor.Yellow, middlePagesSum.ToString());
                }
                else
                {
                    TextUtilities.ColorWriteLine(ConsoleColor.Red, " NOT OK!");
                }
            }

            // Print output
            Console.Write("\n >>> Middle page value sum is ");
            TextUtilities.ColorWriteLine(ConsoleColor.Yellow, middlePagesSum.ToString());
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            int middlePagesSum = 0;

            string[] rows = DataLoader.LoadRowData(5, fullOrSimpleData);
            List<Rule> rules = new List<Rule>();
            List<List<int>> updates = new List<List<int>>();


            #region Input interpretation
            {
                int rowLength = rows.Length;
                int i = 0;
                while (true)
                {
                    if (rows[i] == "") break;
                    string[] split = rows[i].Split('|');
                    rules.Add(new Rule(int.Parse(split[0]), int.Parse(split[1])));
                    i++;
                }
                i++;
                while (i < rowLength)
                {
                    string[] split = rows[i].Split(',');
                    updates.Add((from part in split select int.Parse(part)).ToList());
                    i++;
                }
            }
            #endregion


            // Sort the rule data.
            // For all pages we add a reference to all values that belongs to the left and the right.
            // What this means is that we can later for any number in an update determine if any
            // values to the left or right are incorrectly placed.
            Dictionary<int, NumberWithConditions> nrLookup = new Dictionary<int, NumberWithConditions>();
            foreach (var rule in rules)
            {
                NumberWithConditions con;

                if (nrLookup.TryGetValue(rule.FirstPage, out con))
                {
                    con.ToTheRight.Add(rule.SecondPage);
                }
                else
                {
                    con = new NumberWithConditions();
                    con.ToTheRight.Add(rule.SecondPage);
                    nrLookup.Add(rule.FirstPage, con);
                }


                if (nrLookup.TryGetValue(rule.SecondPage, out con))
                {
                    con.ToTheLeft.Add(rule.FirstPage);
                }
                else
                {
                    con = new NumberWithConditions();
                    con.ToTheLeft.Add(rule.FirstPage);
                    nrLookup.Add(rule.SecondPage, con);
                }
            }

            Stack<List<int>> stackUpdates = new Stack<List<int>>(updates);
            while(stackUpdates.Count > 0)
            {
                List<int> update = stackUpdates.Pop();
                bool jumpOut = false;

                for (int activeElement = 0; activeElement < update.Count; activeElement++)
                {
                    // con contains all values that are required to the left and the right of a value
                    NumberWithConditions con = nrLookup[update[activeElement]];

                    // loops to the left
                    for (int leftIndex = activeElement - 1; leftIndex > -1; leftIndex--)
                    {
                        if (con.ToTheRight.Contains(update[leftIndex])) // if any value to the left belongs on the right
                        {
                            jumpOut = true;
                            stackUpdates.Push(GetOrderedUpdate(update, update[leftIndex], con));
                            break;
                        }
                    }
                    if (jumpOut) break;

                    // loops to the right
                    for (int rightIndex = activeElement + 1; rightIndex < update.Count; rightIndex++)
                    {
                        if (con.ToTheLeft.Contains(update[rightIndex])) // if any value to the right belongs on the right
                        {
                            jumpOut = true;
                            stackUpdates.Push(GetOrderedUpdate(update, update[rightIndex], con));
                            break;
                        }
                    }
                    if (jumpOut) break;
                }
                if (jumpOut) continue;

                Console.Write(string.Join(',', update));
                int previous = middlePagesSum;
                int val = update[update.Count / 2];
                middlePagesSum += val;
                TextUtilities.ColorWrite(ConsoleColor.Green, " OK! ");
                TextUtilities.ColorWrite(ConsoleColor.Yellow, previous.ToString());
                Console.Write(" + ");
                TextUtilities.ColorWrite(ConsoleColor.Green, val.ToString());
                Console.Write(" -> ");
                TextUtilities.ColorWriteLine(ConsoleColor.Yellow, middlePagesSum.ToString());
            }

            // Print output
            Console.Write("\n >>> Middle page value sum is ");
            TextUtilities.ColorWriteLine(ConsoleColor.Yellow, middlePagesSum.ToString());
        }

        List<int> GetOrderedUpdate(List<int> update, int value, NumberWithConditions con)
        {
            int startLength = update.Count;
            update.Remove(value);

            for (int activeElement = 0; activeElement < update.Count; activeElement++)
            {
                bool nextElement = false;
                for (int leftIndex = activeElement - 1; leftIndex > -1; leftIndex--)
                {
                    if (con.ToTheRight.Contains(update[leftIndex]))
                    {
                        nextElement = true;
                        break;
                    }
                }
                if (nextElement) continue;

                for (int rightIndex = activeElement; rightIndex < update.Count; rightIndex++)
                {
                    if (con.ToTheLeft.Contains(update[rightIndex]))
                    {
                        nextElement = true;
                        break;
                    }
                }
                if (nextElement) continue;

                update.Insert(activeElement, value);
            }

            if (update.Count != startLength) return [];
            else return update;
        }


        readonly record struct Rule(int FirstPage, int SecondPage)
        {
            public override string ToString() => FirstPage + "|" + SecondPage;
        }

        public class LeftSizeComparer : IComparer<Rule>
        {
            int IComparer<Rule>.Compare(Rule x, Rule y) => x.FirstPage.CompareTo(y.FirstPage);
        }

        public class RightSizeComparer : IComparer<Rule>
        {
            int IComparer<Rule>.Compare(Rule x, Rule y) => x.SecondPage.CompareTo(y.SecondPage);
        }
    }
}
