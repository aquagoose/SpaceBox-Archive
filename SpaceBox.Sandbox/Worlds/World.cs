using System;
using System.Collections.Generic;
using SpaceBox.Sandbox.Grids;

namespace SpaceBox.Sandbox.Worlds
{
    /// <summary>
    /// Represents a Sandbox world.
    /// </summary>
    public class World
    {
        /// <summary>
        /// All grids that this world contains.
        /// </summary>
        public List<Grid> Grids { get; set; }

        public World()
        {
            Grids = new List<Grid>();
        }

        public void Update()
        {
            foreach (Grid grid in Grids)
                grid.Update();
        }

        public static World CurrentWorld { get; set; }
    }
}