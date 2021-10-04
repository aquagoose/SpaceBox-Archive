using System;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using BepuUtilities.Memory;
using OpenTK.Mathematics;

namespace Cubic.Physics
{
    public static class Physics
    {
        public static BufferPool BufferPool { get; private set; }
        public static Simulation Simulation { get; private set; }

        public static void Initialize()
        {
            BufferPool = new BufferPool();
            Simulation = Simulation.Create(BufferPool, new NarrowPhaseCallbacks(),
                new PoseIntegratorCallbacks(Vector3.Zero), new PositionLastTimestepper());
        }

        public static void Simulate(float deltaTime)
        {
            Simulation.Timestep(deltaTime);
        }

        public static bool Raycast(Vector3 position, Vector3 direction, float maxDist, out RaycastHit hit)
        {
            RaycastHitHandler rayHit = new RaycastHitHandler();
            Simulation.RayCast(position.ToVec3(), direction.ToVec3(), maxDist, ref rayHit);
            hit = rayHit.Hit;
            return hit.Hit;
        }
    }

    public struct RaycastHitHandler : IRayHitHandler
    {
        private bool _hasHit;
        
        public RaycastHit Hit;
        public bool AllowTest(CollidableReference collidable)
        {
            return !_hasHit;
        }

        public bool AllowTest(CollidableReference collidable, int childIndex)
        {
            return !_hasHit;
        }

        public void OnRayHit(in RayData ray, ref float maximumT, float t, in System.Numerics.Vector3 normal, CollidableReference collidable,
            int childIndex)
        {
            _hasHit = true;
            if (collidable.Mobility is CollidableMobility.Dynamic or CollidableMobility.Kinematic)
            {
                BodyReference reference = Physics.Simulation.Bodies.GetBodyReference(collidable.BodyHandle);
                Hit = new RaycastHit
                {
                    Normal = new Vector3(normal.X, normal.Y, normal.Z),
                    Position = new Vector3(reference.Pose.Position.X, reference.Pose.Position.Y, reference.Pose.Position.Z),
                    Rotation = new Quaternion(reference.Pose.Orientation.X, reference.Pose.Orientation.Y,
                        reference.Pose.Orientation.Z, reference.Pose.Orientation.W),
                    Hit = true,
                    Collidable = collidable,
                    ChildIndex = childIndex
                };
            }
            else
            {
                StaticReference reference = Physics.Simulation.Statics.GetStaticReference(collidable.StaticHandle);
                Hit = new RaycastHit
                {
                    Normal = new Vector3(normal.X, normal.Y, normal.Z),
                    Position = new Vector3(reference.Pose.Position.X, reference.Pose.Position.Y, reference.Pose.Position.Z),
                    Rotation = new Quaternion(reference.Pose.Orientation.X, reference.Pose.Orientation.Y,
                        reference.Pose.Orientation.Z, reference.Pose.Orientation.W),
                    Hit = true,
                    Collidable = collidable,
                    ChildIndex = childIndex
                };
            }
        }
    }
}