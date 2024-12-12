using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utilities
{

    public struct DataMap<T>
    {
        public T[,] grid;

        public DataMap(T[,] grid)
        {
            this.grid = grid;
            Dim = new Int2(grid.GetLength(0), grid.GetLength(1));
        }

        public Int2 Dim { get; private set; }

        public bool IsInside(Int2 gridPosition) =>
            gridPosition.x >= 0 && gridPosition.x < Dim.x &&
            gridPosition.y >= 0 && gridPosition.y < Dim.y;

        public List<Int2> GetAllAdjacentPositions(Int2 from)
        {
            // Get adjacent points
            List<Int2> allAdjacents = new List<Int2>();

            Int2 nextPos = from + Int2.Up;
            if (IsInside(nextPos)) allAdjacents.Add(nextPos);
            nextPos = from + Int2.Down;
            if (IsInside(nextPos)) allAdjacents.Add(nextPos);
            nextPos = from + Int2.Right;
            if (IsInside(nextPos)) allAdjacents.Add(nextPos);
            nextPos = from + Int2.Left;
            if (IsInside(nextPos)) allAdjacents.Add(nextPos);

            return allAdjacents;
        }

        public List<T> GetAllAdjacentValues(Int2 from)
        {
            // Get adjacent points
            List<T> allAdjacents = new List<T>();

            Int2 nextPos = from + Int2.Up;
            if (IsInside(nextPos)) allAdjacents.Add(grid[nextPos.x, nextPos.y]);
            nextPos = from + Int2.Down;
            if (IsInside(nextPos)) allAdjacents.Add(grid[nextPos.x, nextPos.y]);
            nextPos = from + Int2.Right;
            if (IsInside(nextPos)) allAdjacents.Add(grid[nextPos.x, nextPos.y]);
            nextPos = from + Int2.Left;
            if (IsInside(nextPos)) allAdjacents.Add(grid[nextPos.x, nextPos.y]);

            return allAdjacents;
        }
    }
}
