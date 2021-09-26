using System;
using System.Collections.Generic;
using BepuPhysics;
using BepuPhysics.Collidables;
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
        
        public List<Block> Blocks;

        public Grid(Vector3 position, Vector3 orientation, GridType gridType, GridSize gridSize)
        {
            Position = position;
            Orientation = orientation;
            GridType = gridType;
            Size = gridSize;
            Blocks = new List<Block>();
        }

        public void GeneratePhysics()
        {
            /*if (Blocks.Count > 1)
            {
                foreach (Block block in Blocks)
                {
                    Physics.Simulation.Statics.Remove(block.StaticHandle);
                    Physics.Simulation.Shapes.Remove();
                }
            }*/

            //Physics.Simulation.Bodies.Remove(BodyReference.Handle);

            foreach (Block block in Blocks)
            {
                Vector3 pos = Position + block.Coord;
                Quaternion orientation = Quaternion.FromEulerAngles(Orientation);

                StaticHandle handle = Physics.Simulation.Statics.Add(new StaticDescription(new System.Numerics.Vector3(pos.X, pos.Y, pos.Z),
                    new System.Numerics.Quaternion(orientation.X, orientation.Y, orientation.Z, orientation.W),
                    new CollidableDescription(Physics.Simulation.Shapes.Add(new Box(2, 2, 2)), 0.1f)));

                block.StaticHandle = handle;
            }
        }
    }
}