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
        private static BufferPool _bufferPool;
        public static Simulation Simulation;

        public static void Initialize()
        {
            _bufferPool = new BufferPool();
            Simulation = Simulation.Create(_bufferPool, new NarrowPhaseCallbacks(),
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
        public RaycastHit Hit;
        public bool AllowTest(CollidableReference collidable)
        {
            return true;
        }

        public bool AllowTest(CollidableReference collidable, int childIndex)
        {
            return true;
        }

        public void OnRayHit(in RayData ray, ref float maximumT, float t, in System.Numerics.Vector3 normal, CollidableReference collidable,
            int childIndex)
        {
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
                    Collidable = collidable
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
                    Collidable = collidable
                };
            }
        }
    }
}