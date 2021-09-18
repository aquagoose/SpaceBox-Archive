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

        public void Update(UIManager manager)
        {
            SpriteBatch batch = manager.SpriteBatch;
            Vector2 scale = manager.UiScale;
            
            switch (DockType)
            {
                case DockType.TopLeft:
                    ScreenPosition = Offset * scale;
                    break;
                case DockType.TopRight:
                    ScreenPosition = new Vector2(batch.Width, 0) + Offset * scale;
                    break;
                case DockType.BottomLeft:
                    ScreenPosition = new Vector2(0, batch.Height) + Offset * scale;
                    break;
                case DockType.BottomRight:
                    ScreenPosition = new Vector2(batch.Width, batch.Height) + Offset * scale;
                    break;
                case DockType.Center:
                    ScreenPosition = new Vector2(batch.Width, batch.Height) / 2f + Offset * scale;
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