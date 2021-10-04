using System.Drawing;
using OpenTK.Mathematics;

namespace Cubic.Utilities
{
    public static class Extensions
    {
        public static Vector2 ToVector2(this Size size)
        {
            return new Vector2(size.Width, size.Height);
        }

        public static System.Numerics.Vector2 ToSystemNumericsVector2(this Vector2 vector2)
        {
            return new System.Numerics.Vector2(vector2.X, vector2.Y);
        }

        public static System.Numerics.Vector3 ToSystemNumericsVector3(this Vector3 vector3)
        {
            return new System.Numerics.Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static System.Numerics.Quaternion ToSystemNumericsQuaternion(this Quaternion quaternion)
        {
            return new System.Numerics.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }
    }
}