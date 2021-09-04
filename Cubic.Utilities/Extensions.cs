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
    }
}