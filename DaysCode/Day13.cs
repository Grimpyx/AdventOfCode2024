using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace AdventOfCode2024.Days
{
    partial class Day13 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // This can be solved with some linear algebra.
            // Essentially, we want to know if we can construct a linear combination of A and B to construct the goal
            // a*v_1  +  b*v_2  =  goal
            // You solve this by doing normal row addition, row multiplication, etc.
            // For that I created CrazyIntMatrix.

            List<ClawGame> clawGames = Parse(DataLoader.LoadRowData(13, fullOrSimpleData));
            List<CrazyIntMatrix> matrices = new List<CrazyIntMatrix>();
            foreach (var game in clawGames)
            {
                int[,] mtrx = new int[,]
                {
                    { game.ButtonA_change.x, game.ButtonA_change.y,},
                    { game.ButtonB_change.x, game.ButtonB_change.y },
                    { game.Goal.x, game.Goal.y}
                };
                matrices.Add(new CrazyIntMatrix(mtrx));
            }

            List<(CrazyIntMatrix m, int a, int b, int cost)> solvableMatrices = new List<(CrazyIntMatrix m, int a, int b, int cost)>();

            foreach (CrazyIntMatrix matrix in matrices)
            {
                if (fullOrSimpleData == DayDataType.Simple)
                {
                    matrix.Print();
                    Console.WriteLine();
                }
                matrix.PerformRowReduction();
                if (fullOrSimpleData == DayDataType.Simple)
                {
                    matrix.Print();
                    Console.WriteLine(new string('-', Console.WindowWidth));
                }

                // If found solution
                if (matrix.matrix[0,0] == matrix.matrix[1,1] && matrix.matrix[1, 1] == 1)
                {
                    int a = matrix.matrix[matrix.matrix.GetLength(0) - 1, 0];
                    int b = matrix.matrix[matrix.matrix.GetLength(0) - 1, 1];

                    solvableMatrices.Add((matrix, a, b, 3*a + b));
                }
            }

            // Print output
            int totalCost = 0;
            foreach (var item in solvableMatrices)
            {
                totalCost += item.cost;
                TextUtilities.CFWLine($"@GraFound solvable. A=@Yel{item.a} @GraB=@Yel{item.b} @DGy| @GraCost @Gre{item.cost}");
            }
            TextUtilities.CFWLine($"@Gra >>> Total token cost: @Yel{totalCost}@Gra");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // The same as first scenario, but now the goal is much much much further away.
            // I had to make another CrazyIntMatrix but for longs, so CrazyLongMatrix

            List<ClawGame> clawGames = Parse(DataLoader.LoadRowData(13, fullOrSimpleData));
            List<CrazyLongMatrix> matrices = new List<CrazyLongMatrix>();
            foreach (var game in clawGames)
            {
                long[,] mtrx = new long[,]
                {
                    { game.ButtonA_change.x, game.ButtonA_change.y,},
                    { game.ButtonB_change.x, game.ButtonB_change.y },
                    { 10000000000000 + game.Goal.x, 10000000000000 + game.Goal.y}
                };
                matrices.Add(new CrazyLongMatrix(mtrx));
            }

            // Stores solved matrices
            List<(CrazyLongMatrix m, long a, long b, long cost)> solvableMatrices = new List<(CrazyLongMatrix m, long a, long b, long cost)>();

            foreach (CrazyLongMatrix matrix in matrices)
            {
                if (fullOrSimpleData == DayDataType.Simple)
                {
                    matrix.Print();
                    Console.WriteLine();
                }
                matrix.PerformRowReduction();
                if (fullOrSimpleData == DayDataType.Simple)
                {
                    matrix.Print();
                    Console.WriteLine(new string('-', Console.WindowWidth));
                }

                // If found solution
                if (matrix.matrix[0, 0] == matrix.matrix[1, 1] && matrix.matrix[1, 1] == 1)
                {
                    long a = matrix.matrix[matrix.matrix.GetLength(0) - 1, 0];
                    long b = matrix.matrix[matrix.matrix.GetLength(0) - 1, 1];

                    solvableMatrices.Add((matrix, a, b, 3 * a + b));
                }
            }

            // Print output
            long totalCost = 0;
            foreach (var item in solvableMatrices)
            {
                totalCost += item.cost;
                TextUtilities.CFWLine($"@GraFound solvable. A=@Yel{item.a} @GraB=@Yel{item.b} @DGy| @GraCost @Gre{item.cost}");
            }
            TextUtilities.CFWLine($"@Gra >>> Total token cost: @Yel{totalCost}@Gra");
        }
       
        record ClawGame(Int2 ButtonA_change, Int2 ButtonB_change, Int2 Goal);

        class ClawGameOldTest
        {
            private Int2 buttonA_change;
            private Int2 buttonB_change;
            private Int2 goal;

            public ClawGameOldTest(Int2 buttonA_change, Int2 buttonB_change, Int2 goal)
            {
                this.buttonA_change = buttonA_change;
                this.buttonB_change = buttonB_change;
                this.goal = goal;
            }

            public Int2 ButtonA_change { get => buttonA_change; }
            public Int2 ButtonB_change { get => buttonB_change; }
            public Int2 Goal { get => goal; }

            public (List<Int2> path, int cost)? FindShortestPath()
            {
                //Int2 currentPosition = Int2.Zero;

                // A* ?
                PriorityQueue<Int2, int> openQueue = new PriorityQueue<Int2, int>();
                Dictionary<Int2, NodeProperties> properties = new Dictionary<Int2, NodeProperties>();
                HashSet<Int2> closedSet = new HashSet<Int2>();

                List<Int2> returnListPath = new List<Int2>();

                // The priority assigned in the open queue represents its f-value
                Int2 startNode = Int2.Zero; //new Node(Int2.Zero, 0, null);
                openQueue.Enqueue(startNode, 0); // First step
                //lowest_f.Add(startNode.Position, 0);

                Int2 q;
                int qf;

                while (openQueue.TryDequeue(out q, out qf))
                {
                    /*Node[] successors = new Node[2];
                    successors[0] = new Node(q.Position + buttonA_change, q.g + 1, q); // Cost is 1 for button A
                    successors[1] = new Node(q.Position + buttonB_change, q.g + 3, q); // Cost is 3 for button B*/

                    closedSet.Add(q);

                    Int2[] successors = new Int2[2];
                    successors[0] = q + buttonA_change; 
                    successors[1] = q + buttonB_change;

                    NodeProperties[] successors_properties = new NodeProperties[2];
                    successors_properties[0] = new NodeProperties(successors[0], qf + 1); // Cost is 1 for button A
                    successors_properties[1] = new NodeProperties(successors[1], qf + 3); // Cost is 3 for button B


                    for (int successorId = 0; successorId < successors.Length; successorId++)
                    {
                        Int2 successor = successors[successorId];

                        if (!closedSet.Contains(successor))
                        {
                            bool isSuccessorInOpenQueue = false;

                            foreach (var openNode in openQueue.UnorderedItems)
                            {
                                if (openNode.Element == successor)
                                {
                                    isSuccessorInOpenQueue = true;
                                    break;
                                }
                            }

                            if (!isSuccessorInOpenQueue)
                            {
                                properties.Add(successor, successors_properties[successorId]);
                                openQueue.Enqueue(successor, successors_properties[successorId].F);
                            }
                        }

                        /*// Try update next successor value
                        if (lowest_f.TryGetValue(successor.Position, out int currentSuccessor_f))
                        {
                            // lowest_f[successor.Position] = currentSuccessor_f;
                            if (successor_f > currentSuccessor_f) continue;
                        }*/
                    }

                }


                // If end was not found, return null
                if (!closedSet.Contains(goal)) return null!;

                // Else we return the path
                Int2 curNode = goal;
                do
                {
                    returnListPath.Add(curNode);
                    curNode = properties[curNode].Parent;
                }
                while (curNode != Int2.Zero); // Start node

                return (returnListPath, properties[goal].F);
            }
        }

        record NodeProperties(Int2 Parent, int F);

        /*class Node
        {
            public Node parent;
            public Int2 position;
            public int cost; // 'g'
            public float f;

        }*/

        List<ClawGame> Parse(string[] data)
        {
            /* Button A: X+94, Y+34
             * Button B: X+22, Y+67
             * Prize: X=8400, Y=5400*/
            List<ClawGame> clawList = new List<ClawGame>();
            Int2[] allInt2sToParse = new Int2[3];

            for (int j = 0; j < data.Length; j+=4)
            {
                for (int i = j; i < j + 3; i++)
                {
                    string row = data[i];
                    string[] commaSplit = row.Split(',');
                    allInt2sToParse[i-j] = new Int2(
                        int.Parse(new string(commaSplit[0].Where(char.IsDigit).ToArray())),
                        int.Parse(new string(commaSplit[1].Where(char.IsDigit).ToArray())));
                }
                clawList.Add(new ClawGame(allInt2sToParse[0], allInt2sToParse[1], allInt2sToParse[2]));
            }
            return clawList;
        }
    }
}
