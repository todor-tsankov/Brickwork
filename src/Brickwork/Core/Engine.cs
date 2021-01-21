using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brickwork.Core
{
    /// <summary>
    /// Responsible for:
    /// Reading the input from the console
    /// Validating the and parsing the input to a int[n,m] matrix,
    /// later to be solved by a IBricksSolver
    /// Printing the result.
    /// </summary>
    public class Engine : IEngine
    {
        // Minimal value of "n" or the number of rows
        private const int MinNCount = 2;

        // Minimal value of "M" or number of columns per row
        private const int MinMCount = 2;

        // Maximum allowed value of "n" or the number of rows
        private const int MaxNCount = 100;

        // Maximum allowed value of "m" or the number of columns per row
        private const int MaxMCount = 100;

        // Starting value for the bricks
        private const int MinBrickNumber = 1;

        // Separator for splitting the input lines
        private const char InputSeparator = ' ';

        // Separator between different bricks when printing
        private const char SameBricksSeparator = '-';

        // Separator between the numbers of the same brick
        private const char DifferentBricksSeparator = '*';

        // The format the numbers will be printed in
        private const string NumbersFormat = "D2";

        // Used to solve the first layer
        private readonly IBricksSolver bricksSolver;

        public Engine(IBricksSolver bricksSolver)
        {
            this.bricksSolver = bricksSolver;
        }

        /// <summary>
        /// Reads input from the console using ReadBrickLayer(int n, int m) method
        /// then validates the input using the ValidBricks(int[,] layer, int n, int m) method
        /// Uses the IBricks solver to solve the layer and prints the result on the console.
        /// </summary>
        public void Start()
        {
            try
            {
                // First line of input as an array
                var dimensions = this.ReadLineNumbers();

                // The number of rows
                var n = dimensions[0];

                // The number of columns
                var m = dimensions[1];

                // The first layer that has to be solved
                var firstLayer = this.ReadBrickLayer(n, m);

                // Check if the input is valid
                if (!this.ValidBricks(firstLayer, n, m))
                {
                    Console.WriteLine("Invalid input!");
                    return;
                }

                // Holds the solved layer(second layer)
                var solved = this.bricksSolver.Solve(firstLayer);

                // Prints the result
                if (solved != null)
                {
                    this.PrintResult(n, m, solved);
                }
                else
                {
                    Console.WriteLine("There is no solution!");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Part of the input contains something that is not a number!");
            }
        }

        /// <summary>
        /// Prints the solved or second layer with symbols
        /// between each brick.
        /// </summary>
        /// <param name="n">Number of rows.</param>
        /// <param name="m">Number of columns.</param>
        /// <param name="solved">The solved layer.</param>
        private void PrintResult(int n, int m, int[,] solved)
        {
            // Holds the lines with numbers to be printed
            var numberLines = new string[n];

            // iterate over each row of the solved layer
            for (int row = 0; row < n; row++)
            {
                // Holds the string for the current line
                var sb = new StringBuilder();
                sb.Append(DifferentBricksSeparator);

                // iterate over the current row of the solved layer
                for (int col = 0; col < m; col++)
                {
                    sb.Append(solved[row, col].ToString(NumbersFormat));

                    // If the next number is the same row different than current (they are different bricks)
                    // and we should append DifferentBricksSeparator
                    // else the SameBricksSeparator
                    if (col + 1 < m && solved[row, col + 1] != solved[row, col])
                    {
                        sb.Append(DifferentBricksSeparator);
                    }
                    else if (col + 1 < m)
                    {
                        sb.Append(SameBricksSeparator);
                    }
                }

                sb.Append(DifferentBricksSeparator);
                numberLines[row] = sb.ToString().TrimEnd();
            }

            // The length of the longest string line
            var maxLength = numberLines.Max(x => x.Length);

            // The string for the bottom and the top of the matrix
            var bottomAndTop = new string(DifferentBricksSeparator, maxLength);

            // Holds everything that should be printed
            var completeSb = new StringBuilder();

            completeSb.AppendLine("\nThe result is:\n");
            completeSb.AppendLine(bottomAndTop);

            // iterate over all lines with numbers that have to be printed
            for (int i = 0; i < numberLines.Length; i++)
            {
                // holds the current number line
                var currentLine = numberLines[i];
                completeSb.AppendLine(currentLine);

                // we only print line with separator symbols if the current line is not the last
                if (i == numberLines.Length - 1)
                {
                    break;
                }

                // holds the result string for the current line with separators
                var lineSb = new StringBuilder();

                // the next row/line with numbers
                var nextLine = numberLines[i + 1];

                // iterate over the whole length of the currentLine
                for (int y = 0; y < currentLine.Length; y++)
                {
                    // If the  current element is digit and is equal to the
                    // corresponding element on the next line.
                    if (currentLine[y] == nextLine[y] && char.IsDigit(currentLine[y]))
                    {
                        // If the next element is digit too and is equal to the
                        // corresponding element on the next line.
                        if ((char.IsDigit(currentLine[y + 1]) && currentLine[y + 1] == nextLine[y + 1])
                            || !char.IsDigit(currentLine[y + 1]))
                        {
                            lineSb.Append(SameBricksSeparator);
                        }
                        else
                        {
                            lineSb.Append(DifferentBricksSeparator);
                        }
                    }
                    else
                    {
                        lineSb.Append(DifferentBricksSeparator);
                    }
                }

                var resultLine = lineSb.ToString().TrimEnd();
                completeSb.AppendLine(resultLine);
            }

            completeSb.AppendLine(bottomAndTop);
            Console.WriteLine(completeSb.ToString().TrimEnd());
        }

        /// <summary>
        /// Validates the a given layer is valid.
        /// </summary>
        /// <param name="layer">The int[,] matrix holding the layer with its bricks.</param>
        /// <param name="n">Number of rows.</param>
        /// <param name="m">Number of columns.</param>
        /// <returns>Whether the given layer is valid.</returns>
        private bool ValidBricks(int[,] layer, int n, int m)
        {
            // Holds information about how many times each brick number appeared
            // to check if there are bricksspanning more than 3 numbers
            var bricks = new Dictionary<int, int>();

            // The maximum number a brick can have
            var maxBrickNumber = n * m / 2;

            // Holds info whether the number of row or columns is uneven
            var uneven = n % 2 != 0 || m % 2 != 0;

            // Hold info whether the number of rows or columns is outside
            // the allowed range
            var nOutOfRange = n < MinNCount || n > MaxNCount;
            var mOutOfRange = m < MinMCount || n > MaxMCount;

            // If any criteria of the ones above is false the whole layer should be marked
            // invalid an any additional validation is meaningless
            if (uneven || nOutOfRange || mOutOfRange)
            {
                return false;
            }

            // Iterates over the whole matrix layer at every [row, column]
            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < m; col++)
                {
                    // Holds info whether the current number is valid (false by default)
                    var valid = false;

                    // The number at the current index - [row, col]
                    var current = layer[row, col];

                    // Checks whether there is a corresponding number up, down, left or right
                    // so that the current and this number form a brick
                    if ((row - 1 >= 0 && layer[row - 1, col] == current)
                        || (row + 1 < n && layer[row + 1, col] == current)
                        || (col - 1 >= 0 && layer[row, col - 1] == current)
                        || (col + 1 < m && layer[row, col + 1] == current))
                    {
                        valid = true;
                    }

                    // If the bircks dictionary does'n yet conatain
                    // the current brick number we should initialize it
                    if (!bricks.ContainsKey(current))
                    {
                        bricks[current] = 0;
                    }

                    // Increase the number of appearances for the current brick
                    bricks[current]++;

                    // If any criteria is false the whole layer should be marked
                    // invalid an any additional validation is meaningless
                    if (current < MinBrickNumber || current > maxBrickNumber || !valid || bricks[current] > 2)
                    {
                        return false;
                    }
                }
            }

            // If we succeeded to get to this point the layer is valid
            return true;
        }

        /// <summary>
        /// // Reads a layer form the console, according to
        /// the number of rows and columns - n, m.
        /// </summary>
        /// <param name="n">number of rows.</param>
        /// <param name="m">number of columns.</param>
        /// <returns>the read layer.</returns>
        private int[,] ReadBrickLayer(int n, int m)
        {
            var layer = new int[n, m];

            for (int row = 0; row < n; row++)
            {
                var currentRow = this.ReadLineNumbers();

                // If the given row has an invalid number of values - break
                if (currentRow.Length != m)
                {
                    break;
                }

                for (int col = 0; col < m; col++)
                {
                    layer[row, col] = currentRow[col];
                }
            }

            return layer;
        }

        /// <summary>
        /// Reads a single line from the console
        /// Splits it by space and parses each
        /// element of the result array to int.
        /// </summary>
        /// <returns>The result int[] array.</returns>
        private int[] ReadLineNumbers()
        {
            return Console.ReadLine()
                            .Split(InputSeparator, StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToArray();
        }
    }
}
