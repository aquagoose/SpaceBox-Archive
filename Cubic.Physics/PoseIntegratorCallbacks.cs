using OpenTK.Mathematics;
using BepuPhysics;
using Vec3 = System.Numerics.Vector3;

namespace Cubic.Physics
{
    public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
    {
        public Vec3 Gravity;
        private Vec3 _gravityDt;

        public PoseIntegratorCallbacks(Vector3 gravity) : this()
        {
            Gravity = gravity.ToVec3();
        }
        
        public void Initialize(Simulation simulation)
        {
            
        }

        public void PrepareForIntegration(float dt)
        {
            _gravityDt = Gravity * dt;
        }

        public void IntegrateVelocity(int bodyIndex, in RigidPose pose, in BodyInertia localInertia, int workerIndex,
            ref BodyVelocity velocity)
        {
            if (localInertia.InverseMass > 0)
                velocity.Linear = velocity.Linear + _gravityDt;
        }

        public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;
    }
}