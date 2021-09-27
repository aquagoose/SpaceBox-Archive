using System;
using System.Diagnostics;
using Cubic.Physics;
using Cubic.Utilities;
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
        public Player Player;

        // The actual camera used to get the view & projection matrices.
        public Camera CameraBase;
        
        public PlaceCube PlaceCube { get; }

        public Vector3 Forward => CameraBase.Forward;
        public Vector3 Backward => -CameraBase.Forward;
        public Vector3 Right => CameraBase.Right;
        public Vector3 Left => -CameraBase.Right;
        public Vector3 Up => CameraBase.Up;
        public Vector3 Down => -CameraBase.Up;

        public Vector3 Position;
        // X: Yaw, Y: Pitch, Z: Roll(bot.net)
        public Vector3 Rotation;

        public int PlaceCubeDistance { get; set; } = 6;

        public PlayerCamera(Vector3 position, Vector3 rotation, float aspectRatio, float fov, float near, float far)
        {
            Position = position;
            Rotation = rotation;
            CameraBase = new Camera(position, rotation, aspectRatio, fov, near, far);
            PlaceCube = new PlaceCube();
        }

        public void Update()
        {
            CameraBase.Position = Position;
            CameraBase.Yaw = Rotation.X;
            CameraBase.Pitch = Rotation.Y;
            CameraBase.Roll = Rotation.Z;

            PlaceCube.Position = Position + Forward * PlaceCubeDistance;
            
            //Console.WriteLine(PlaceCube.Position);

            if (Physics.Raycast(Position, Forward, PlaceCubeDistance, out RaycastHit hit))
            {
                Grid lookingGrid = null;
                Block lookingBlock = null;
                
                //Console.WriteLine(hit.Collidable.StaticHandle);
                
                foreach (Grid grid in World.Instance.Grids)
                {
                    foreach (Block block in grid.Blocks)
                    {
                        if (block.StaticHandle == hit.Collidable.StaticHandle)
                            lookingBlock = block;
                    }
                    if (lookingBlock != null)
                        //Console.WriteLine("yee");
                        lookingGrid = grid;

                    Vector3 cubePos = lookingGrid.Position + lookingBlock.Coord * 2;
                    PlaceCube.Position = cubePos + hit.Normal * 2;
                }
                
                Debug.Assert(lookingGrid != null, "lookingGrid != null");

                if (Input.IsMouseButtonPressed(MouseButton.Left))
                {
                    // Invert the matrices to convert from world space to model space, to get our correct rotation &
                    // position matrices.
                    Matrix4 invertModel = Matrix4.Invert(Matrix4.CreateFromQuaternion(hit.Rotation) *
                                                         Matrix4.CreateTranslation(hit.Position));
                    Matrix4 invertPlacecube = Matrix4.Invert(Matrix4.CreateFromQuaternion(PlaceCube.Rotation) *
                                                             Matrix4.CreateTranslation(PlaceCube.Position));
                    
                    // Get the position vector.
                    Vector4 vec = (invertModel - invertPlacecube).Row3;
                    // Normalize the position vector and remove any floating-point inaccuracies.
                    Vector3 normalizedVec = new Vector3((int) vec.X, (int) vec.Y, (int) vec.Z).Normalized();
                    lookingGrid.Blocks.Add(new Block(lookingBlock.Coord + normalizedVec));
                    lookingGrid.GeneratePhysics();
                }
            }
            else
            {
                if (Input.IsMouseButtonPressed(MouseButton.Left))
                {
                    Console.WriteLine("Presse");
                    Grid grid = new Grid(PlaceCube.Position, PlaceCube.Rotation.ToEulerAngles(),
                        GridType.Static, GridSize.Medium);
                    grid.Blocks.Add(new Block(Vector3.Zero));
                    grid.GeneratePhysics();
                    World.Instance.Grids.Add(grid);
                }
            }
        }
    }
}