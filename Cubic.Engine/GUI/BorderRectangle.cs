﻿using System.Drawing;
using Cubic.Engine.Render;
using OpenTK.Mathematics;

namespace Cubic.Engine.GUI
{
    /// <summary>
    /// Represents a rectangle with the given border thickness
    /// </summary>
    public class BorderRectangle : UIElement
    {
        private Texture2D _texture;

        public BorderRectangle(UIManager manager, Position position, Size size, int borderWidth, Color color) : base(manager, position,
            size, color)
        {
            Color[] pixels = new Color[size.Width * size.Height];
            // Generate pixels around the border.
            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    if (x < borderWidth || x >= size.Width - borderWidth || y < borderWidth ||
                        y >= size.Height - borderWidth)
                        pixels[y * size.Width + x] = Color.White;
                    else
                        pixels[y * size.Width + x] = Color.Transparent;
                }
            }

            _texture = new Texture2D(size.Width, size.Height, pixels, mipmap: false, autoDispose: false);
        }
        protected internal override void Draw()
        {
            Vector2 scale = IgnoreScale ? Vector2.One : UiManager.UiScale;
            SpriteBatch.Draw(_texture, Position.ScreenPosition, Color, Rotation, Origin, scale);
        }

        public override void Dispose()
        {
            _texture.Dispose();
            
            base.Dispose();
        }
    }
}