using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day13 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            List<ClawGame> clawGames = Parse(DataLoader.LoadRowData(13, fullOrSimpleData));
            Console.WriteLine("Banana");
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }

        class ClawGame
        {
            private Int2 buttonA_change;
            private Int2 buttonB_change;
            private Int2 goal;

            public ClawGame(Int2 buttonA_change, Int2 buttonB_change, Int2 goal)
            {
                this.buttonA_change = buttonA_change;
                this.buttonB_change = buttonB_change;
                this.goal = goal;
            }

            public int FindShortestPath()
            {
                //Int2 currentPosition = Int2.Zero;

                // A* ?
                PriorityQueue<Node, int> openQueue = new PriorityQueue<Node, int>();
                Dictionary<Int2, int> lowest_f = new Dictionary<Int2, int>();
                HashSet<Int2> closedSet = new HashSet<Int2>();

                // The priority assigned in the open queue represents its f-value
                Node startNode = new Node(Int2.Zero, 0, null);
                openQueue.Enqueue(startNode, 0); // First step
                lowest_f.Add(startNode.Position, 0);

                Node q;
                int qf;

                while (openQueue.TryDequeue(out q, out qf))
                {
                    Node[] successors = new Node[2];
                    successors[0] = new Node(q.Position + buttonA_change, q.g + 1, q); // Cost is 1 for button A
                    successors[1] = new Node(q.Position + buttonB_change, q.g + 3, q); // Cost is 3 for button B

                    for (int i = 0; i < successors.Length; i++)
                    {
                        Node successor = successors[i];
                        int successor_h = 0;
                        int successor_f = successor.g + successor_h;

                        // Try update next successor value
                        if (lowest_f.TryGetValue(successor.Position, out int currentSuccessor_f))
                        {
                            // lowest_f[successor.Position] = currentSuccessor_f;
                            if (successor_f > currentSuccessor_f) continue;
                        }
                    }

                }


            }
        }

        record Node(Int2 Position, int g, Node Parent);

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
