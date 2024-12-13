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
                PriorityQueue<Int2, int> frontier = new PriorityQueue<Int2, int>();
                frontier.Enqueue(Int2.Zero, 0); // First step

            }
        }

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
