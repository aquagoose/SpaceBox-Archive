using System;
using System.Collections.Generic;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Cubic.Physics;
using Cubic.Utilities;
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
        public Quaternion Orientation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public GridType GridType { get; set; }
        public GridSize Size { get; set; }
        
        public BodyHandle BodyHandle { get; set; }
        public TypedIndex ShapeIndex { get; set; }
        public BigCompound Compound;

        public List<Block> Blocks;

        private bool _bodyHandleMade;

        public Grid(Vector3 position, Quaternion orientation, GridType gridType, GridSize gridSize)
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
            using (CompoundBuilder builder = new CompoundBuilder(Physics.Simulation.BufferPool, Physics.Simulation.Shapes, 10))
            {
                foreach (Block block in Blocks)
                {
                    //Console.WriteLine(block.Coord.ToSystemNumericsVector3() * 2);
                    builder.Add(new Box(2, 2, 2), new RigidPose(block.Coord.ToSystemNumericsVector3() * 2), 0.1f);
                }

                builder.BuildDynamicCompound(out Buffer<CompoundChild> children, out BodyInertia inertia);
                builder.Reset();
                Compound = new BigCompound(children, builder.Shapes, Physics.Simulation.BufferPool);
                if (!_bodyHandleMade)
                {
                    ShapeIndex = Physics.Simulation.Shapes.Add(Compound);
                    BodyHandle = Physics.Simulation.Bodies.Add(BodyDescription.CreateDynamic(
                        new RigidPose(Position.ToSystemNumericsVector3(), Orientation.ToSystemNumericsQuaternion()),
                        inertia, new CollidableDescription(ShapeIndex, 0.1f), new BodyActivityDescription(0.01f)));
                    _bodyHandleMade = true;
                }
                else
                {
                    Physics.Simulation.Shapes.Remove(ShapeIndex);
                    ShapeIndex = Physics.Simulation.Shapes.Add(Compound);
                    Physics.Simulation.Bodies.SetShape(BodyHandle, ShapeIndex);
                }
            }
        }

        public void Update()
        {
            BodyReference reference = Physics.Simulation.Bodies.GetBodyReference(BodyHandle);
            Position = new Vector3(reference.Pose.Position.X, reference.Pose.Position.Y, reference.Pose.Position.Z);
            Orientation = new Quaternion(reference.Pose.Orientation.X, reference.Pose.Orientation.Y,
                reference.Pose.Orientation.Z, reference.Pose.Orientation.W);
        }
    }
}