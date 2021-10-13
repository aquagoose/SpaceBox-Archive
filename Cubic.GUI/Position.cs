using System;
using Cubic.Render;
using OpenTK.Mathematics;

namespace Cubic.GUI
{
    /// <summary>
    /// Represents a Position on screen, that adapts to screen size & scaling.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// The actual screen position of the element.
        /// </summary>
        /// <remarks>You cannot set this value, instead, change the <see cref="Offset"/>.</remarks>
        public Vector2 ScreenPosition { get; private set; }

        /// <summary>
        /// The actual X-position of this element on screen. This returns the X coordinate of the <see cref="ScreenPosition"/>.
        /// </summary>
        public float X => ScreenPosition.X;
        /// <summary>
        /// The actual Y-position of this element on screen. This returns the Y coordinate of the <see cref="ScreenPosition"/>.
        /// </summary>
        public float Y => ScreenPosition.Y;
        
        /// <summary>
        /// The offset of this element, relative to it's <see cref="DockType"/>.
        /// </summary>
        public Vector2 Offset;
        
        /// <summary>
        /// Where on the screen this element is located. This value will adjust automatically based on the screen size.
        /// </summary>
        public DockType DockType;

        /// <summary>
        /// Create a new Position with the given <see cref="DockType"/> and <see cref="Offset"/>.
        /// </summary>
        /// <param name="dockType">Where on the screen this element should be automatically positioned.</param>
        /// <param name="offset">The offset of the element.</param>
        public Position(DockType dockType, Vector2 offset)
        {
            DockType = dockType;
            Offset = offset;
        }

        /// <summary>
        /// Create a new Position with the specified X and Y coordinates.
        /// </summary>
        /// <param name="position">The absolute position of this element.</param>
        /// <remarks>The <see cref="DockType"/> is set to TopLeft.</remarks>
        public Position(Vector2 position) : this(DockType.TopLeft, position) { }

        /// <summary>
        /// Create a new Position with the specified X and Y coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of this element.</param>
        /// <param name="y">The Y coordinate of this element.</param>
        /// <remarks>The <see cref="DockType"/> is set to TopLeft.</remarks>
        public Position(float x, float y) : this(new Vector2(x, y)) { }

        /// <summary>
        /// Update the Position of the element. You MUST do this in order for the element to function properly.
        /// You may run it more than once.
        /// </summary>
        /// <param name="manager">The UI manager, used for scaling.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Update(UIManager manager)
        {
            // Get our spritebatch for screen size, and scale for UI scale.
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