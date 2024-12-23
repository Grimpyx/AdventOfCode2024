﻿using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day7 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // 7579994664925 too high
            // 7579994664753 correct!
            // I had accidentally included results that were 'incomplete'.
            // For example:  6: 1 2 3 4 would show up as successful, but only because it stopped at 1+2+3, without including the 4.
            // In the correct cases we have to consider using all numbers in the list.

            string[] data = DataLoader.LoadRowData(7, fullOrSimpleData);
            Equation[] equations = new Equation[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                string[] split = data[i].Split(": ");
                long goal = long.Parse(split[0]);
                List<int> ints = split[1].Split(' ').Select(int.Parse).ToList();
                equations[i] = new Equation(goal, ints);
            }

            long total = 0;
            foreach (var equation in equations)
            {
                Console.WriteLine();
                equation.CalculateP1();
                if (equation.hasReachedGoal)
                {
                    total += equation.goal;
                    TextUtilities.CFWLine($"@Gre > @GraFound an equation with at least one path. Adding @Yel{equation.goal} @Grato total.");
                }
                else
                    TextUtilities.CFWLine($"@Red > @GraFound no solution.");
            }
            TextUtilities.CFWLine($"@Gra >>> The total: @Yel{total}");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // Part 2 is easy.
            // It's the same as part 1, but we add another case. However, it takes way longer than before!

            // Interpret data
            string[] data = DataLoader.LoadRowData(7, fullOrSimpleData);
            Equation[] equations = new Equation[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                string[] split = data[i].Split(": ");
                long goal = long.Parse(split[0]);
                List<int> ints = split[1].Split(' ').Select(int.Parse).ToList();
                equations[i] = new Equation(goal, ints);
            }

            // Calculate all equations
            long total = 0;
            foreach (var equation in equations)
            {
                Console.WriteLine();
                equation.CalculateP2();
                if (equation.hasReachedGoal)
                {
                    total += equation.goal;
                    TextUtilities.CFWLine($"@Gre > @GraFound an equation with at least one path. Adding @Yel{equation.goal} @Grato total.");
                }
                else
                    TextUtilities.CFWLine($"@Red > @GraFound no solution.");
            }
            TextUtilities.CFWLine($"@Gra >>> The total: @Yel{total}");
        }

        class Equation
        {
            public long goal;
            public List<int> numbers;
            public bool hasReachedGoal = false;

            public Equation(long goal, List<int> numbers)
            {
                this.goal = goal;
                this.numbers = numbers;
            }

            // Figure out the order of operators.
            // Updates hasReachedGoal if successful.
            public void CalculateP1()
            {
                TextUtilities.CFWLine(ToCFWString());

                // Create two queues.
                // We use 'queue' to loop through.
                // We add two to 'nextQueue' for each element dequeued from 'queue'; one for each operator
                PriorityQueue<List<(int index, Operator op)>, long> queue = new PriorityQueue<List<(int index, Operator op)>, long>();
                PriorityQueue<List<(int index, Operator op)>, long> nextQueue = new PriorityQueue<List<(int index, Operator op)>, long>();

                // Enqueue the first node
                queue.Enqueue([(0, Operator.None)], numbers[0]); // [new NumberWithOperator(numbers[0], Operator.None)]

                //TextUtilities.CFWLine("@Gra" + "000 : ".PadRight(23) + ": " + -numbers[0]);
                //Console.WriteLine(new string('-', 40));

                // Loop through all the numbers in the list
                for (int numberIndex = 1; numberIndex < numbers.Count; numberIndex++)
                {
                    // Add two more elements
                    while (queue.TryDequeue(out var indexAndHistory, out long priority))
                    {
                        // New histories for the two nodes
                        List<(int index, Operator op)> indexAndHistory1 = new List<(int index, Operator op)>(indexAndHistory);
                        List<(int index, Operator op)> indexAndHistory2 = new List<(int index, Operator op)>(indexAndHistory);

                        // New index is the latest index + 1
                        int nextIndex = indexAndHistory[^1].index + 1;

                        // Add addition case
                        indexAndHistory1.Add((nextIndex, Operator.Addition));
                        nextQueue.Enqueue(indexAndHistory1, priority + numbers[nextIndex]); // Represents addition

                        // Add multiplication case
                        indexAndHistory2.Add((nextIndex, Operator.Multiplication));
                        nextQueue.Enqueue(indexAndHistory2, priority * numbers[nextIndex]); // Represents multiplication
                    }

                    // If the last iteration, break such that we don't clear the queue
                    if (numberIndex == numbers.Count - 1)
                        break;

                    // Update nextQueue and queue for the next iteration
                    queue = nextQueue;
                    nextQueue = new PriorityQueue<List<(int index, Operator op)>, long>();
                }

                // Print and evaluate the queue if it contains the goal
                foreach (var item in nextQueue.UnorderedItems)
                {
                    if (item.Priority == goal)
                    {
                        TextUtilities.CFWLine("@Gra" + item.Element[^1].index.ToString("000") + (" : " + string.Join("", item.Element.Select(x => (char)x.op))).PadRight(20) + " : @Gre" + item.Priority + "@Gra");
                        hasReachedGoal = true;
                    }
                    // Uncomment this if you want to see everything
                    //else
                    //TextUtilities.CFWLine("@Gra" + item.Element[^1].index.ToString("000") + (" : " + string.Join("", item.Element.Select(x => (char)x.op))).PadRight(20) + " : " + item.Priority + "@Gra");
                }
                Console.WriteLine(new string('-', 40));
            }
            
            // Figure out the order of operators.
            // Updates hasReachedGoal if successful.
            public void CalculateP2()
            {
                TextUtilities.CFWLine(ToCFWString());

                // Create two queues.
                // We use 'queue' to loop through.
                // We add two to 'nextQueue' for each element dequeued from 'queue'; one for each operator
                PriorityQueue<List<(int index, Operator op)>, long> queue = new PriorityQueue<List<(int index, Operator op)>, long>();
                PriorityQueue<List<(int index, Operator op)>, long> nextQueue = new PriorityQueue<List<(int index, Operator op)>, long>();

                // Enqueue the first node
                queue.Enqueue([(0, Operator.None)], numbers[0]); // [new NumberWithOperator(numbers[0], Operator.None)]

                //TextUtilities.CFWLine("@Gra" + "000 : ".PadRight(23) + ": " + -numbers[0]);
                //Console.WriteLine(new string('-', 40));

                // Loop through all the numbers in the list
                for (int numberIndex = 1; numberIndex < numbers.Count; numberIndex++)
                {
                    // Add two more elements
                    while (queue.TryDequeue(out var indexAndHistory, out long priority))
                    {
                        // New histories for the two nodes
                        List<(int index, Operator op)> indexAndHistory1 = new List<(int index, Operator op)>(indexAndHistory);
                        List<(int index, Operator op)> indexAndHistory2 = new List<(int index, Operator op)>(indexAndHistory);
                        List<(int index, Operator op)> indexAndHistory3 = new List<(int index, Operator op)>(indexAndHistory);

                        // New index is the latest index + 1
                        int nextIndex = indexAndHistory[^1].index + 1;

                        // Add addition case
                        indexAndHistory1.Add((nextIndex, Operator.Addition));
                        nextQueue.Enqueue(indexAndHistory1, priority + numbers[nextIndex]); // Represents addition

                        // Add multiplication case
                        indexAndHistory2.Add((nextIndex, Operator.Multiplication));
                        nextQueue.Enqueue(indexAndHistory2, priority * numbers[nextIndex]); // Represents multiplication

                        // Part 2 special case
                        // Add concat case
                        indexAndHistory3.Add((nextIndex, Operator.Concat)); // because we use negative numbers
                        nextQueue.Enqueue(indexAndHistory3, Concat(priority, numbers[nextIndex])); // Represents concatenation
                    }

                    // If the last iteration, break such that we don't clear the queue
                    if (numberIndex == numbers.Count - 1)
                        break;

                    // Update nextQueue and queue for the next iteration
                    queue = nextQueue;
                    nextQueue = new PriorityQueue<List<(int index, Operator op)>, long>();
                }

                // Print and evaluate the queue if it contains the goal
                foreach (var item in nextQueue.UnorderedItems)
                {
                    if (item.Priority == goal)
                    {
                        TextUtilities.CFWLine("@Gra" + item.Element[^1].index.ToString("000") + (" : " + string.Join("", item.Element.Select(x => (char)x.op))).PadRight(20) + " : @Gre" + item.Priority + "@Gra");
                        hasReachedGoal = true;
                    }
                    // Uncomment this if you want to see everything
                    //else
                    //TextUtilities.CFWLine("@Gra" + item.Element[^1].index.ToString("000") + (" : " + string.Join("", item.Element.Select(x => (char)x.op))).PadRight(20) + " : " + item.Priority + "@Gra");
                }
                Console.WriteLine(new string('-', 40));
            }

            public string ToCFWString()
            {
                return $"@Whi///) {goal} ".PadRight(22) + "@Yel" + string.Join("@DGy,@Yel", numbers);
            }
        }
        static long Concat(long a, long b)
        {
            // 25 || 623 = 25623 = 25000 + 623 = 25 * 10^(623.digitAmount) + 623 = a * 10^(b.digitAmount) + b
            return a * (long)Math.Pow(10, (int)Math.Log10(b) + 1) + b;
        }

        /*record struct NumberWithOperator(long Number, Operator Op)
        {
            public readonly long Evaluate(long otherInt)
            {
                return Op switch
                {
                    Operator.None => Number,
                    Operator.Multiplication => Number * otherInt,
                    Operator.Addition => Number + otherInt,
                    Operator.Concat => Concat(Number, otherInt),
                    _ => -1,
                };
            }
        }*/

        enum Operator
        {
            None,
            Multiplication = '*',
            Addition = '+',
            Concat = '|'
        }
    }
}
