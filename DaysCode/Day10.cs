using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day10 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            TopographicMap topographicMap = ParseMap(DataLoader.LoadRowData(10, fullOrSimpleData));

            /*for (int y = 0; y < topographicMap.Map.Dim.y; y++)
            {
                for (int x = 0; x < topographicMap.Map.Dim.x; x++)
                {
                    int height = topographicMap.Map.grid[x, y];

                    if ()
                }
            }*/

            topographicMap.ExploreAllTrails();
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }

        public record Trailhead(List<Int2> positions);

        public TopographicMap ParseMap(string[] rowsOfData)
        {
            int[,] grid = new int[rowsOfData[0].Length, rowsOfData.Length];
            List<Trailhead> trailList = new List<Trailhead>();
            for (int y = 0; y < rowsOfData.Length; y++)
            {
                for (int x = 0; x < rowsOfData[0].Length; x++)
                {
                    grid[x,y] = rowsOfData[y][x];
                    if (grid[x, y] == 0) trailList.Add(new Trailhead([new Int2(x,y)]));
                }
            }
            return new TopographicMap(new DataMap<int>(grid), trailList);
        }
    
        public class TopographicMap
        {
            private readonly DataMap<int> map = new DataMap<int>();
            private readonly List<Trailhead> trailheads = new List<Trailhead>();

            public TopographicMap(DataMap<int> map, List<Trailhead> trailheads)
            {
                this.map = map;
                this.trailheads = trailheads;
            }

            public void ExploreAllTrails()
            {
                foreach (var trail in trailheads)
                {
                    
                }
            }

            public DataMap<int> Map => map;
            public List<Trailhead> Trailheads => trailheads;
        }
    }
}
