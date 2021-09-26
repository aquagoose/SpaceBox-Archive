using OpenTK.Mathematics;

namespace Cubic.Physics
{
    internal static class Extensions
    {
        public static System.Numerics.Vector3 ToVec3(this Vector3 vec)
        {
            return new System.Numerics.Vector3(vec.X, vec.Y, vec.Z);
        }
    }
}