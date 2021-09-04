using System.Drawing;
using Cubic.Render;
using Cubic.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Spacebox.Scenes
{
    public class IntroScene : Scene
    {
        private Texture2D _logo;
        
        public IntroScene(SpaceboxGame game) : base(game) { }

        public override void Initialize()
        {
            base.Draw();
            
            GL.ClearColor(Color.Black);
            
            _logo = new Texture2D("Content/Textures/Images/ismlogo.png", autoDispose: false);
        }

        public override void Draw()
        {
            base.Draw();
            
            Game.SpriteBatch.Begin();

            Vector2 imgScale = new Vector2(Game.SpriteBatch.Width / (float) _logo.Width,
                Game.SpriteBatch.Height / (float) _logo.Height);
            Game.SpriteBatch.Draw(_logo, new Vector2(Game.SpriteBatch.Width, Game.SpriteBatch.Height) / 2f, Color.White,
                0, _logo.Size.ToVector2() / 2f, new Vector2(imgScale.X < imgScale.Y ? imgScale.X : imgScale.Y));
            
            Game.SpriteBatch.End();
        }

        public override void Unload()
        {
            base.Unload();
            
            _logo.Dispose();
        }
    }
}