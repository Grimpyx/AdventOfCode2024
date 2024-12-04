using AdventOfCode2024.Utilities;
using System.Numerics;

namespace AdventOfCode2024.Days
{
    class Day4 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // Load and interpret data
            string[] rowData = DataLoader.LoadRowData(4, fullOrSimpleData);
            Element[,] grid = new Element[rowData[0].Length, rowData.Length]; // grid[x,y], counting from the top left
            for (int x = 0; x < rowData[0].Length; x++) // x
            {
                for (int y = 0; y < rowData.Length; y++) // y
                {
                    grid[y, x] = (Element)rowData[y][x];
                }
            }

            DataMap<Element> map = new DataMap<Element>(grid);

            // Print data
            PrintGrid(map);

            // StartProcess
            int total = 0;
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {

                }
                Console.WriteLine();
            }
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }

        enum Element
        {
            X = 'X',
            M = 'M',
            A = 'A',
            S = 'S'
        }

        private void PrintGrid(DataMap<Element> map)
        {
            Element[,] grid = map.grid;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Console.Write(grid[x, y]);
                }
                Console.WriteLine();
            }
        }

        private (int x, int y)[] GetAllAdjacent(DataMap<Element> map, Int2 origin,  Element target)
        {
            Int2 minLimit = new Int2(1, 1);
            Int2 maxLimit = map.Dim - (2 * minLimit);

            Int2 GetRotatedVector(int variable) // 0-8, otherwise is %8
            {
                //round(sin(𝜋𝑥/ 4))⁆
            }
        }

        struct DataMap<T>
        {
            public T[,] grid;

            public DataMap(T[,] grid)
            {
                this.grid = grid;
                Dim = new Int2(grid.GetLength(0), grid.GetLength(1));
            }

            public Int2 Dim { get; private set; }
        }
    }

   
}
