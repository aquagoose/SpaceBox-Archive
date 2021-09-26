using System;
using System.Collections.Generic;
using Cubic.Physics;
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
        public List<Grid> Grids;

        public World()
        {
            Grids = new List<Grid>();
        }

        public static World Instance { get; set; }
    }
}