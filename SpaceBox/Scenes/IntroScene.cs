using System;
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

        private float _alpha;
        private Color _color;

        public float _startTime;
        
        public IntroScene(SpaceboxGame game) : base(game) { }

        public override void Initialize()
        {
            base.Draw();
            
            GL.ClearColor(Color.Black);
            
            _logo = new Texture2D("Content/Textures/Images/ismlogo.png", autoDispose: false);

            _startTime = Time.ElapsedSeconds;
            _alpha = 0;
        }

        public override void Update()
        {
            base.Update();

            // The amount of time it takes for the image to fade, in seconds.
            const float fadeTime = 0.2f;
            // Times[0] is the time until the image starts to fade in after scene has loaded
            // Times[1] is the time the image will display for until fade out
            // Times[2] is the time the scene will change after the image has faded out.
            int[] times = { 1, 3, 2 };

            if (Time.ElapsedSeconds - _startTime - times[0] - times[1] > times[2])
                Game.SetScene(new MenuScene(Game));
            else if (Time.ElapsedSeconds - _startTime - times[0] > times[1])
                _alpha = MathHelper.Lerp(1, 0, (Time.ElapsedSeconds - _startTime - times[0] - times[1]) / fadeTime);
            else if (Time.ElapsedSeconds - _startTime > times[0])
                _alpha = MathHelper.Lerp(0, 1, (Time.ElapsedSeconds - _startTime - times[0]) / fadeTime);

            _color = Color.FromArgb((int) (_alpha * 255f), Color.White);
        }

        public override void Draw()
        {
            base.Draw();
            
            Game.SpriteBatch.Begin();

            Vector2 imgScale = new Vector2(Game.SpriteBatch.Width / (float) _logo.Width,
                Game.SpriteBatch.Height / (float) _logo.Height);
            Game.SpriteBatch.Draw(_logo, new Vector2(Game.SpriteBatch.Width, Game.SpriteBatch.Height) / 2f, _color,
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