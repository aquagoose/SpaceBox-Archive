using System;
using System.Drawing;
using Cubic.Render;
using Cubic.Utilities;

namespace Cubic.GUI
{
    /// <summary>
    /// Represents a rectangle with a solid colour.
    /// </summary>
    public class FillRectangle : UIElement
    {
        // The rectangle's texture.
        private Texture2D _texture;

        public FillRectangle(UIManager manager, Position position, Size size, Color color) : base(manager, position,
            size, color)
        {
            // Here, we create a 1x1 texture, as it's just a solid colour and doing this saves memory.
            // Then in the draw method we just stretch it out to the desired size!
            _texture = new Texture2D(1, 1, new[] { Color.White }, mipmap: false);
        }
        protected internal override void Draw()
        {
            SpriteBatch.Draw(_texture, Position.ScreenPosition, Color, Rotation, Origin, Size.ToVector2());
        }
    }
}