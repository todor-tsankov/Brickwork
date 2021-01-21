using System;
using System.Linq;

using Brickwork.Core;

namespace Brickwork
{
    public class Program
    {
        /// <summary>
        /// Start the IEngine with a corresponding IBricksSolver.
        /// </summary>
        public static void Main()
        {
            var bricksSolver = new BricksSolver() as IBricksSolver;
            var engine = new Engine(bricksSolver) as IEngine;

            engine.Start();
        }
    }
}
