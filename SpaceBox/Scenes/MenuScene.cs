using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceBox.GUI;
using Font = Cubic.GUI.Fonts.Font;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace Spacebox.Scenes
{
    public class MenuScene : Scene
    {
        private Font _font;
        
        public MenuScene(SpaceboxGame game) : base(game) { }

        private string[] _resolutions;
        private int _currentRes;

        private MenuImageSystem _system;

        private Texture2D _bg;
        private Position _pos;

        public override void Initialize()
        {
            base.Initialize();

            //_font = new Font("Content/Fonts/arial.ttf", Game.SpriteBatch);
            
            GL.ClearColor(Color.FromArgb(178, 178, 178));

            _bg = new Texture2D("Content/Textures/Images/Menu/spacebox-blurred.jpg");
            _pos = new Position(DockType.Center, -new Vector2(_bg.Width, _bg.Height) / 2 + new Vector2(300, 50));
            //Game.UiManager.Add("bg",
            //    new Cubic.GUI.Image(Game.UiManager, _bg,
            //        new Position(DockType.Center, -new Vector2(_bg.Width, _bg.Height) / 2 + new Vector2(300, 50)), Vector2.One, Color.White));
        }

        public override void Update()
        {
            base.Update();

            _pos.Update(Game.SpriteBatch);
        }

        public override void Draw()
        {
            base.Draw();
            
            Game.SpriteBatch.Begin();
            
            Game.SpriteBatch.Draw(_bg, _pos.ScreenPosition, Color.White, 0, Vector2.Zero, Vector2.One);
            
            Game.SpriteBatch.End();
            
            //Game.UiManager.Draw();

            //_font.DrawString(50, "Hello", new Vector2(100), Vector2.One, Color.White);
        }
    }
}