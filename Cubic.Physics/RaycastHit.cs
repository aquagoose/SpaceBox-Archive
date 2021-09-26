using BepuPhysics.Collidables;
using OpenTK.Mathematics;

namespace Cubic.Physics
{
    public struct RaycastHit
    {
        public Vector3 Position { get; internal set; }
        public Quaternion Rotation { get; internal set; }
        public Vector3 Normal { get; internal set; }
        public CollidableReference Collidable { get; internal set; }
        internal bool Hit { get; set; }
    }
}