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
using Vector2 = OpenTK.Mathematics.Vector2;

namespace Spacebox.Scenes
{
    public class MenuScene : Scene
    {
        public MenuScene(SpaceboxGame game) : base(game) { }

        private string[] _resolutions;
        private int _currentRes;

        private MenuImageSystem _system;

        public override void Initialize()
        {
            base.Initialize();
            
            GL.ClearColor(Color.FromArgb(178, 178, 178));

            Texture2D _bg = new Texture2D("Content/Textures/Images/Menu/spacebox-blurred.jpg");
            Game.UiManager.Add("bg",
                new Cubic.GUI.Image(Game.UiManager, _bg,
                    new Position(DockType.Center, -new Vector2(_bg.Width, _bg.Height) / 2 + new Vector2(300, 50)), Vector2.One, Color.White));
        }

        public override void Update()
        {
            base.Update();

            
        }
    }
}