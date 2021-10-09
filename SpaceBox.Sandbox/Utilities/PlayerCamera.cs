using System;
using System.Diagnostics;
using Cubic.Physics;
using Cubic.Utilities;
using Cubic.Windowing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceBox.Sandbox.Grids;
using SpaceBox.Sandbox.Worlds;

namespace SpaceBox.Sandbox.Utilities
{
    /// <summary>
    /// A player camera deals with block placement, etc.
    /// </summary>
    public class PlayerCamera
    {
        public Player Player { get; }

        // The actual camera used to get the view & projection matrices.
        public Camera CameraBase { get; }
        
        public PlaceCube PlaceCube { get; }

        public Vector3 Forward => CameraBase.Forward;
        public Vector3 Backward => -CameraBase.Forward;
        public Vector3 Right => CameraBase.Right;
        public Vector3 Left => -CameraBase.Right;
        public Vector3 Up => CameraBase.Up;
        public Vector3 Down => -CameraBase.Up;

        public Vector3 Position
        {
            get => CameraBase.Position;
            set => CameraBase.Position = value;
        }
        
        public Quaternion Rotation
        {
            get => CameraBase.Rotation;
            set => CameraBase.Rotation = value;
        }

        public int PlaceCubeDistance { get; set; } = 6;

        public PlayerCamera(Vector3 position, Quaternion rotation, float aspectRatio, float fov, float near, float far)
        {
            CameraBase = new Camera(position, rotation, aspectRatio, fov, near, far);
            Position = position;
            Rotation = rotation;
            PlaceCube = new PlaceCube();
        }

        public void Update()
        {
            PlaceCube.Position = Position + Forward * PlaceCubeDistance;

            if (Physics.Raycast(Position, Forward, PlaceCubeDistance, out RaycastHit hit))
            {
                // TODO: Use quadtree for efficiency
                // Loop through each grid in the world to see which grid player is looking at.
                foreach (Grid grid in World.CurrentWorld.Grids)
                {
                    // Since each grid has a handle and the RaycastHit returns a collidable handle, we can just check
                    // to see if they match. If they do, that's the grid we're looking at!
                    if (grid.BodyHandle == hit.Collidable.BodyHandle)
                    {
                        // As we're using a BigCompound, each block within the grid has a child index, with the origin
                        // block being 0 etc. The child index increases in the order we place the blocks, so it can be
                        // used to determine the exact block we are looking at.
                        Block currentBlock = grid.Blocks[hit.ChildIndex];
                        PlaceCube.Position = grid.Position + grid.Orientation * currentBlock.Coord * 2 + hit.Normal * 2;
                        PlaceCube.Rotation = grid.Orientation;

                        if (Input.IsMouseButtonPressed(MouseButton.Left))
                        {
                            // Invert the matrices to convert from world space to model space, to get our correct rotation &
                            // position matrices.
                            Matrix4 invertModel = Matrix4.Invert(Matrix4.CreateFromQuaternion(hit.Rotation) *
                                                                 Matrix4.CreateTranslation(hit.Position + currentBlock.Coord * 2));
                            Matrix4 invertPlacecube = Matrix4.Invert(Matrix4.CreateFromQuaternion(grid.Orientation) *
                                                                     Matrix4.CreateTranslation(grid.Position +
                                                                         currentBlock.Coord * 2 + hit.Normal * 2));
                            
                            // Get the position vector.
                            Vector4 vec = (invertModel - invertPlacecube).Row3;
                            // Normalize the position vector and remove any floating-point inaccuracies.
                            Vector3 normalizedVec = new Vector3((int) vec.X, (int) vec.Y, (int) vec.Z).Normalized();
                            grid.Blocks.Add(new Block(currentBlock.Coord + normalizedVec));
                            grid.GeneratePhysics();
                            
                        }
                        else if (Input.IsMouseButtonPressed(MouseButton.Right))
                        {
                            // Remove the block from the grid.
                            grid.Blocks.RemoveAt(hit.ChildIndex);
                            // If no blocks are left...
                            if (grid.Blocks.Count < 1)
                            {
                                // Remove the grid's shape and body from memory,
                                Physics.Simulation.Shapes.Remove(grid.ShapeIndex);
                                Physics.Simulation.Bodies.Remove(grid.BodyHandle);
                                // Then remove the grid from the world.
                                World.CurrentWorld.Grids.Remove(grid);
                                // Break because we're done for this frame.
                                break;
                            }
                            // Re-generate physics if required.
                            grid.GeneratePhysics();
                        }
                        // We break here because there's no point scanning through grids we're not looking at.
                        break;
                    }
                }
            }
            // If the player is not currently looking at a grid...
            else if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                // Create a new grid
                Grid grid = new Grid(PlaceCube.Position, PlaceCube.Rotation,
                    GridType.Dynamic, GridSize.Medium);
                // Add a block
                grid.Blocks.Add(new Block(Vector3.Zero));
                // And generate physics!
                grid.GeneratePhysics();
                World.CurrentWorld.Grids.Add(grid);
            }
        }
    }
}