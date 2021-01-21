namespace Brickwork.Core
{
    /// <summary>
    /// Interface which every BricksSolver should implement
    /// Helps to change the current one with new one.
    /// </summary>
    public interface IBricksSolver
    {
        /// <param name="firstLayer">Recieves the first layer of bricks.</param>
        /// <returns>The solution or null if no solution exists.</returns>
        int[,] Solve(int[,] firstLayer);
    }
}
