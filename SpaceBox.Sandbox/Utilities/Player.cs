using BepuPhysics;
using BepuPhysics.Collidables;
using Cubic.Physics;
using Cubic.Utilities;
using Cubic.Windowing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceBox.Sandbox.Utilities
{
    public class Player
    {
        public PlayerCamera Camera;

        public BodyHandle BodyHandle;

        public Vector3 Position;
        public Quaternion Orientation;
        
        public Player(Vector3 position, Quaternion orientation, float aspectRatio, float fov, float near = 0.1f, float far = 1000f)
        {
            Position = position;
            Orientation = orientation;

            Camera = new PlayerCamera(position, orientation, aspectRatio, fov, near, far);
            Capsule capsule = new Capsule(1, 2);
            capsule.ComputeInertia(100, out BodyInertia inertia);
            BodyHandle = Physics.Simulation.Bodies.Add(BodyDescription.CreateDynamic(
                new RigidPose(position.ToSystemNumericsVector3(), orientation.ToSystemNumericsQuaternion()), inertia,
                new CollidableDescription(Physics.Simulation.Shapes.Add(capsule), 0.1f),
                new BodyActivityDescription(0.01f)));
        }

        public void Update()
        {
            BodyReference body = Physics.Simulation.Bodies.GetBodyReference(BodyHandle);

            body.Pose.Orientation = Orientation.ToSystemNumericsQuaternion();
            
            if (Input.IsKeyDown(Keys.W))
                body.ApplyImpulse(new System.Numerics.Vector3(1), System.Numerics.Vector3.Zero);
        }
    }
}