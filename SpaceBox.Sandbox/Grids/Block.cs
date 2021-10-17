using BepuPhysics;
using OpenTK.Mathematics;

namespace SpaceBox.Sandbox.Grids
{
    public class Block
    {
        /// <summary>
        /// The name of the block
        /// </summary>
        public string Name { get; set; }
        
        /*/// <summary>
        /// The minimum coordinate of this block on the grid.
        /// </summary>
        public Vector3 MinCoord { get; set; }
        /// <summary>
        /// The maximum coordinate of this block on the grid.
        /// </summary>
        public Vector3 MaxCoord { get; set; }*/
        
        public Vector3 Coord { get; set; }

        /*public Block(Vector3 minCoord, Vector3 maxCoord)
        {
            MinCoord = minCoord;
            MaxCoord = maxCoord;
        }*/

        public Block(Vector3 coord)
        {
            Coord = coord;
        }
    }
}