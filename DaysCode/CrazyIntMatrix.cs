using AdventOfCode2024.Utilities;

namespace AdventOfCode2024.Days
{
    partial class Day13
    {
        public class CrazyIntMatrix
        {
            public int[,] matrix;
            private int[,] matrix_original;

            public int NumberOfRows => matrix.GetLength(1);
            public int NumberOfColumns => matrix.GetLength(0);

            public void ResetMatrix()
            {
                matrix = matrix_original;
            }

            public CrazyIntMatrix(int[,] matrix)
            {
                this.matrix = matrix;
                matrix_original = matrix;
            }
            public CrazyIntMatrix(Int2[] vectors)
            {
                matrix = new int[vectors.Length,2];

                for (int i = 0; i < vectors.Length; i++)
                {
                    matrix[i, 0] = vectors[i].x;
                    matrix[i, 1] = vectors[i].y;
                }
                matrix_original = matrix;
            }

            public void PerformRowReduction()
            {
                // Make matrix smaller if possible
                // To avoid integers overflowing
                for (int i = 0; i < NumberOfRows; i++)
                {
                    ReduceRowNumberSizes(i);
                }

                // Reduce column.
                // For a 2 row 3 column matrix, it reduces to
                // 1 0 A
                // 0 1 B
                for (int i = 0; i < NumberOfRows; i++)
                {
                    ReduceColumn(i, i);
                    ReduceRowNumberSizes(i);
                }
                for (int i = 0; i < NumberOfRows; i++)
                {
                    ReduceRowNumberSizes(i);
                }
            }

            public void ReduceRowNumberSizes(int row)
            {
                int gcd = NumberUtilities.GreatestCommonDivisor(GetRow(row).Where(x=> x != 0).ToArray());

                for (int col = 0; col < NumberOfColumns; col++)
                {
                    if (matrix[col, row] == 0) continue;
                    matrix[col, row] /= gcd;
                }
            }

            // Reduces a matrix
            // 1 1 2 2
            // 1 6 4 2
            // Performing ReduceColumn(0,0) leads to:
            // 1 1 2 2
            // 0 5 2 0
            private void ReduceColumn(int column, int rowToKeep)
            {
                int[] columnNumbers = GetColumn(column);
                int lcm = NumberUtilities.LeastCommonMultiple(columnNumbers);

                // Make rows same size by using the least common multiple
                for (int row = 0; row < NumberOfRows; row++)
                {
                    MultiplyRow(row, lcm / matrix[column, row]);
                    MultiplyRow(row, lcm / matrix[column, row]);
                }

                // Remove from all other rows except the row to keep
                MultiplyRow(rowToKeep, -1); // multiply by -1 so that AddRows lead to a subtraction
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (row == rowToKeep) continue;

                    AddRows(rowToKeep, row);
                }
                MultiplyRow(rowToKeep, -1);
            }

            // Row operations (multiply)
            void MultiplyRow(int rowNr, int factor)
            {
                for (int x = 0; x < NumberOfColumns; x++)
                {
                    matrix[x, rowNr] *= factor;
                }
            }

            // Row operations (add)
            void AddRows(int fromRow, int toRow)
            {
                for (int x = 0; x < NumberOfColumns; x++)
                {
                    matrix[x, toRow] += matrix[x, fromRow];
                }
            }

            public int[] GetColumn(int column)
            {
                int[] array = new int[NumberOfRows];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = matrix[column, i];
                }
                return array;
            }

            public int[] GetRow(int row)
            {
                int[] array = new int[NumberOfColumns];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = matrix[i, row];
                }
                return array;
            }

            public void Print()
            {
                for (int y = 0; y < NumberOfRows; y++)
                {
                    for (int x = 0; x < NumberOfColumns; x++)
                    {
                        string toPrint = matrix[x, y].ToString().PadRight(10);
                        TextUtilities.CFW("@Yel" + toPrint);
                    }
                    Console.WriteLine();
                }
            }
        }

        public class CrazyLongMatrix
        {
            public long[,] matrix;
            private long[,] matrix_original;

            public int NumberOfRows => matrix.GetLength(1);
            public int NumberOfColumns => matrix.GetLength(0);

            public void ResetMatrix()
            {
                matrix = matrix_original;
            }

            public CrazyLongMatrix(long[,] matrix)
            {
                this.matrix = matrix;
                matrix_original = matrix;
            }

            public void PerformRowReduction()
            {
                // Make matrix smaller if possible
                // To avoid integers overflowing
                for (int i = 0; i < NumberOfRows; i++)
                {
                    ReduceRowNumberSizes(i);
                }

                // Reduce column.
                // For a 2 row 3 column matrix, it reduces to
                // 1 0 A
                // 0 1 B
                for (int i = 0; i < NumberOfRows; i++)
                {
                    ReduceColumn(i, i);
                    ReduceRowNumberSizes(i);
                }
                for (int i = 0; i < NumberOfRows; i++)
                {
                    ReduceRowNumberSizes(i);
                }
            }

            public void ReduceRowNumberSizes(int row)
            {
                long gcd = NumberUtilities.GreatestCommonDivisorLong(GetRow(row).Where(x => x != 0).ToArray());

                for (int col = 0; col < NumberOfColumns; col++)
                {
                    if (matrix[col, row] == 0) continue;
                    matrix[col, row] /= gcd;
                }
            }

            // Reduces a matrix
            // 1 1 2 2
            // 1 6 4 2
            // Performing ReduceColumn(0,0) leads to:
            // 1 1 2 2
            // 0 5 2 0
            private void ReduceColumn(int column, int rowToKeep)
            {
                long[] columnNumbers = GetColumn(column);
                long lcm = NumberUtilities.LeastCommonMultipleLong(columnNumbers);

                // Make rows same size by using the least common multiple
                for (int row = 0; row < NumberOfRows; row++)
                {
                    MultiplyRow(row, lcm / matrix[column, row]);
                    MultiplyRow(row, lcm / matrix[column, row]);
                }

                // Remove from all other rows except the row to keep
                MultiplyRow(rowToKeep, -1); // multiply by -1 so that AddRows lead to a subtraction
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (row == rowToKeep) continue;

                    AddRows(rowToKeep, row);
                }
                MultiplyRow(rowToKeep, -1);
            }

            // Row operations (multiply)
            void MultiplyRow(int rowNr, long factor)
            {
                for (int x = 0; x < NumberOfColumns; x++)
                {
                    matrix[x, rowNr] *= factor;
                }
            }

            // Row operations (add)
            void AddRows(int fromRow, int toRow)
            {
                for (int x = 0; x < NumberOfColumns; x++)
                {
                    matrix[x, toRow] += matrix[x, fromRow];
                }
            }

            public long[] GetColumn(int column)
            {
                long[] array = new long[NumberOfRows];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = matrix[column, i];
                }
                return array;
            }

            public long[] GetRow(int row)
            {
                long[] array = new long[NumberOfColumns];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = matrix[i, row];
                }
                return array;
            }

            public void Print()
            {
                for (int y = 0; y < NumberOfRows; y++)
                {
                    for (int x = 0; x < NumberOfColumns; x++)
                    {
                        string toPrint = matrix[x, y].ToString().PadRight(10);
                        TextUtilities.CFW("@Yel" + toPrint);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
