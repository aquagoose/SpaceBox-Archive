using System;
using Cubic.Render;
using OpenTK.Mathematics;

namespace Cubic.GUI
{
    public class Position
    {
        public Vector2 ScreenPosition { get; private set; }

        public float X => ScreenPosition.X;
        public float Y => ScreenPosition.Y;
        
        public Vector2 Offset;
        public DockType DockType;

        public Position(DockType dockType, Vector2 offset)
        {
            DockType = dockType;
            Offset = offset;
        }

        public Position(Vector2 position) : this(DockType.TopLeft, position) { }

        public Position(float x, float y) : this(new Vector2(x, y)) { }

        internal void Update(SpriteBatch batch)
        {
            switch (DockType)
            {
                case DockType.TopLeft:
                    ScreenPosition = Offset;
                    break;
                case DockType.TopRight:
                    ScreenPosition = new Vector2(batch.Width, 0) + Offset;
                    break;
                case DockType.BottomLeft:
                    ScreenPosition = new Vector2(0, batch.Height) + Offset;
                    break;
                case DockType.BottomRight:
                    ScreenPosition = new Vector2(batch.Width, batch.Height) + Offset;
                    break;
                case DockType.Center:
                    ScreenPosition = new Vector2(batch.Width, batch.Height) / 2f + Offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum DockType
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center
    }
}