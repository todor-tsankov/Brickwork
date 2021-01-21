using Brickwork.Models;

namespace Brickwork.Core
{
    public class BricksSolver : IBricksSolver
    {
        // Holds the number of rows
        private int n;

        // holds the number of columns
        private int m;

        // Holds the first layer
        private int[,] firstLayer;

        /// <summary>
        /// Wraps the RecursiveSolve() method to set
        /// the necessary fileds and give the necessary parameters.
        /// </summary>
        /// <param name="firstLayer">The layer that has to be solved.</param>
        /// <returns>The solved or second layer.</returns>
        public int[,] Solve(int[,] firstLayer)
        {
            this.n = firstLayer.GetLength(0);
            this.m = firstLayer.GetLength(1);

            this.firstLayer = firstLayer;
            var secondLayer = new int[this.n, this.m];

            return this.RecursiveSolve(secondLayer);
        }

        /// <summary>
        /// Solves the first layer by trying all possible valid brick placements.
        /// Starts at index [0, 0] and finishes at [n - 1, m - 1] and from 1 to
        /// n * m / 2 for Bricks' numbers.
        /// </summary>
        /// <param name="secondLayer">The solved layer.</param>
        /// <param name="counter">Counter for the current number of the brcik.</param>
        /// <param name="row">The current row in the matrix.</param>
        /// <param name="col">The current column in the matrix.</param>
        /// <returns>The second layer(completely or partially solved).</returns>
        private int[,] RecursiveSolve(int[,] secondLayer, int counter = 1, int row = 0, int col = 0)
        {
            // If the current column goes outside the array
            // we should increase the row and reset the column to 0
            if (col == this.m)
            {
                col = 0;
                row++;

                // If the current row goes outside the matrix we either have found
                // a solution or there is no such one in this variation
                // This is the "Bottom" of the recursion
                if (row == this.n)
                {
                    if (counter - 1 != this.n * this.m / 2)
                    {
                        return null;
                    }

                    return secondLayer;
                }
            }

            // Holds the coordinates of the start of the new brick
            // we have to place
            var brickStarIndex = new Index(row, col);

            // If it' possible to place a brick on the current index and right
            // make a copy of the layer and set the new brick. Try to find a
            // solution using the same method. If solution is not possible this way
            // continue with the possibilities
            if (this.Right(brickStarIndex, secondLayer))
            {
                var copy = secondLayer.Clone() as int[,];

                copy[brickStarIndex.Row, brickStarIndex.Col] = counter;
                copy[brickStarIndex.Row, brickStarIndex.Col + 1] = counter;

                var result = this.RecursiveSolve(copy, counter + 1, row, col + 1);

                if (result != null)
                {
                    return result;
                }
            }

            // Samee as above but for down
            if (this.Down(brickStarIndex, secondLayer))
            {
                var copy = secondLayer.Clone() as int[,];

                copy[brickStarIndex.Row, brickStarIndex.Col] = counter;
                copy[brickStarIndex.Row + 1, brickStarIndex.Col] = counter;

                var result = this.RecursiveSolve(copy, counter + 1, row, col + 1);

                if (result != null)
                {
                    return result;
                }
            }

            // If you didn't find solution up to this point it's because
            // The index to the right or down is already taken so continue
            // searching at the next column
            return this.RecursiveSolve(secondLayer, counter, row, col + 1);
        }

        /// <summary>
        /// Checks whether you can place the second part of
        /// a brick bolow the first one considering if it's outside
        /// the matrix, whether there's place for a brick and
        /// if there's brick exactly underneath on the first layer.
        /// </summary>
        /// <param name="index">The index for the first part of the brick.</param>
        /// <param name="secondLayer">The solved layer.</param>
        /// <returns>Whether you can place a brick on that index and down.</returns>
        private bool Down(Index index, int[,] secondLayer)
        {
            return index.Row + 1 < this.n
                && this.firstLayer[index.Row, index.Col] != this.firstLayer[index.Row + 1, index.Col]
                && secondLayer[index.Row, index.Col] == 0
                && secondLayer[index.Row + 1, index.Col] == 0;
        }

        /// <summary>
        /// Checks whether you can place the second part of
        /// a brick right of the first one considering if it's outside
        /// the matrix, whether there's place for a brick and
        /// if there's brick exactly underneath on the first layer.
        /// </summary>
        /// <param name="index">The index for the first part of the brick.</param>
        /// <param name="secondLayer">The solved layer.</param>
        /// <returns>Whether you can place a brick on that index and right.</returns>
        private bool Right(Index index, int[,] secondLayer)
        {
            return index.Col + 1 < this.m
                && this.firstLayer[index.Row, index.Col] != this.firstLayer[index.Row, index.Col + 1]
                && secondLayer[index.Row, index.Col] == 0
                && secondLayer[index.Row, index.Col + 1] == 0;
        }
    }
}
