using System;
using System.Collections.Generic;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Cubic.Physics;
using OpenTK.Mathematics;

namespace SpaceBox.Sandbox.Grids
{
    public class Grid
    {
        /// <summary>
        /// The grid's name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The position of the grid, if appropriate.
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// The orientation of the grid, if appropriate.
        /// </summary>
        public Vector3 Orientation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public GridType GridType { get; set; }
        public GridSize Size { get; set; }
        
        //public BodyReference BodyReference { get; set; }
        public BigCompound Compound;
        
        public List<Block> Blocks;

        public Grid(Vector3 position, Vector3 orientation, GridType gridType, GridSize gridSize)
        {
            Position = position;
            Orientation = orientation;
            GridType = gridType;
            Size = gridSize;
            Blocks = new List<Block>();
            Compound = new BigCompound();
        }

        public void GeneratePhysics()
        {
            
        }
    }
}