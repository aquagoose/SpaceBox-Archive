using System.Drawing;
using Cubic.Engine.Render;
using OpenTK.Mathematics;

namespace Cubic.Engine.GUI
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
            Vector2 scale = IgnoreScale ? Vector2.One : UiManager.UiScale;
            SpriteBatch.Draw(_texture, Position.ScreenPosition, Color, Rotation, Origin, Scale * scale);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _texture.Dispose();
        }
    }
}