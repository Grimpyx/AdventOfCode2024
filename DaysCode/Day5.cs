using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventOfCode2024.Days
{
    class Day5 : IDayChallenge
    {
        public class Brimbam : IComparable<Brimbam> // Interface for custom comparison
        {
            public int Number { get; init; }
            public NumberWithConditions Conditions { get; }

            public Brimbam(int number)
            {
                Number = number;
                Conditions = new NumberWithConditions();
            }

            public Brimbam(int number, NumberWithConditions conditions)
            {
                Number = number;
                Conditions = conditions;
            }

            /*int IComparer<Brimbam>.Compare(Brimbam? x, Brimbam? y)
            {
                if (x == null || y == null) return 0;

                if (x.Conditions.ToTheRight.Contains(y.Number)) return -1;
                if (x.Conditions.ToTheLeft.Contains(y.Number)) return 1;

                if (y.Conditions.ToTheRight.Contains(x.Number)) return 1;
                if (y.Conditions.ToTheLeft.Contains(x.Number)) return -1;

                return 0;
            }*/

            // Specifies how a list of these objects would be sorted.
            int IComparable<Brimbam>.CompareTo(Brimbam? other)
            {
                var x = this;
                var y = other;

                if (x == null || y == null) return 0;

                if (x.Conditions.ToTheRight.Contains(y.Number)) return -1;  // if y belongs to the right of x   ->   x<y  -> -1
                if (x.Conditions.ToTheLeft.Contains(y.Number)) return 1;    // if y belongs to the left of x    -> y<x    -> 1

                if (y.Conditions.ToTheRight.Contains(x.Number)) return 1;   // if x belongs to the right of y   -> y<x    -> 1
                if (y.Conditions.ToTheLeft.Contains(x.Number)) return -1;   // if x belongs to the left of y    ->   x<y  -> -1

                return 0; // If there are no rules specifying how they should be ordered, treat them as Equal
            }

            public override string ToString()
            {
                return Number.ToString();
            }
        }

        public record class NumberWithConditions
        {
            public HashSet<int> ToTheLeft { get; }
            public HashSet<int> ToTheRight { get; }

            public NumberWithConditions()
            {
                ToTheLeft = new HashSet<int>();
                ToTheRight = new HashSet<int>();
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
                NumberWithConditions? con;

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
                NumberWithConditions? con;

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

            // Find all that are wrong and get them as list of list of brimbrams
            List<List<Brimbam>> brimbams = GetAllIncorrect(updates, nrLookup);

            Console.WriteLine("Sorting using custom IComparer<T>:");
            foreach (var brimbam in brimbams)
            {
                string printString = string.Join(',', brimbam.Select(x => x.Number));
                Console.Write( "/ Unsorted: (" + printString + ")\n");
                Brimbam[] unsorted = brimbam.ToArray();
                brimbam.Sort();
                printString = string.Join(',', brimbam.Select(x => x.Number));
                Console.Write("\\        => (");
                for (int i = 0; i < brimbam.Count; i++)
                {
                    if (i != 0) Console.Write(',');
                    bool inTheMiddle = i == brimbam.Count / 2;
                    bool isMoved = unsorted[i].Number != brimbam[i].Number;

                    if (inTheMiddle && isMoved) Console.ForegroundColor = ConsoleColor.Magenta;
                    else if (inTheMiddle) Console.ForegroundColor = ConsoleColor.Green;
                    else if (isMoved) Console.ForegroundColor = ConsoleColor.Yellow;
                    else Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(brimbam[i].Number);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                // Add to total:
                middlePagesSum += brimbam[brimbam.Count / 2].Number;

                // Write the total after each 'Update'
                Console.Write(") total += ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(brimbam[brimbam.Count / 2].Number);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" = ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(middlePagesSum);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(new string('-', 100));
            }
            Console.WriteLine();

            // print result
            Console.Write(" >>> Total: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(middlePagesSum);
            Console.ForegroundColor = ConsoleColor.Gray;

        }

        List<List<Brimbam>> GetAllIncorrect(List<List<int>> updates, Dictionary<int, NumberWithConditions> nrLookup)
        {
            // The list of new brimbams to be returned
            List<List<Brimbam>> incorrect = new List<List<Brimbam>>();

            // For each update
            for (int updateId = 0; updateId < updates.Count; updateId++)
            {
                List<int> update = updates[updateId];
                bool jumpOut = false; // Dictates when we've found an incorrect one

                // Loop through the elements of the update.
                // We check if there are any numbers to the left or right that cannot be there.
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
                            break;
                        }
                    }

                    if (!jumpOut) // Skip looping thgrough the right side if we already found something on the left side
                    {
                        // loops to the right
                        for (int rightIndex = activeElement + 1; rightIndex < update.Count; rightIndex++)
                        {
                            if (con.ToTheLeft.Contains(update[rightIndex])) // if any value to the right belongs on the right
                            {
                                jumpOut = true;
                                break;
                            }
                        }
                    }
                }

                // Create the entry in the returning list
                if (jumpOut)
                {
                    List<Brimbam> temp = new List<Brimbam>();
                    foreach (int nr in update)
                    {
                        temp.Add(new Brimbam(nr, nrLookup[nr]));
                    }
                    incorrect.Add(temp);
                    continue; // move on to the next update
                }
            }
            return incorrect;
        }



        /*List<int> GetOrderedUpdate(Dictionary<int, NumberWithConditions> nrLookup)//(List<int> update, int value, NumberWithConditions con)
        {
            Console.WriteLine("Found an 'update' that was invalid. Trying to reorder.");

            // Start building a new list where all the placement should be ok in order.
            List<int> newUpdate = new List<int>();
            return newUpdate;    
        }*/

            /*
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
            else return update;*/
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
