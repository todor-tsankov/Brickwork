namespace Brickwork.Models
{
    /// <summary>
    /// Used to store a given index or [row, column]
    /// inside a int[,] matrix.
    /// </summary>
    public class Index
    {
        public Index(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }

        public int Row { get; set; }

        public int Col { get; set; }
    }
}
