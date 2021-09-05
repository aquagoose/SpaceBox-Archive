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
    }
}