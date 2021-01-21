namespace Brickwork.Core
{
    /// <summary>
    /// Interface each engine should implement
    /// Helps to change the engine if needed.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Each engine should have a start method
        /// to start reading input.
        /// </summary>
        void Start();
    }
}
