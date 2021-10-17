using OpenTK.Mathematics;

namespace Cubic.Engine.Physics
{
    internal static class Extensions
    {
        public static System.Numerics.Vector3 ToVec3(this Vector3 vec)
        {
            return new System.Numerics.Vector3(vec.X, vec.Y, vec.Z);
        }
    }
}