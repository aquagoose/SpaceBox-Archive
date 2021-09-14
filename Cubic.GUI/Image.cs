using System.Drawing;
using Cubic.Render;
using Cubic.Utilities;
using OpenTK.Mathematics;

namespace Cubic.GUI
{
    public class Image : UIElement
    {
        private Texture2D _texture;

        public Vector2 Scale;

        public Image(UIManager manager, Texture2D texture, Position position, Vector2 scale, Color color) : base(
            manager, position, texture.Size, color)
        {
            _texture = texture;
            Scale = scale;
        }
        
        protected internal override void Draw()
        {
            SpriteBatch.Draw(_texture, Position.ScreenPosition, Color, Rotation, Origin, Scale);
        }
    }
}