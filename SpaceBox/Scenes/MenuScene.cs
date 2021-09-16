using System;
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
using Font = Cubic.GUI.Fonts.Font;

namespace Spacebox.Scenes
{
    public class MenuScene : Scene
    {
        public MenuScene(SpaceboxGame game) : base(game) { }

        private string[] _resolutions;
        private int _currentRes;

        private MenuImageSystem _system;

        private Font _font;

        public override void Initialize()
        {
            base.Initialize();
            
            GL.ClearColor(Color.FromArgb(178, 178, 178));

            _font = new Font("Content/Fonts/inversionz.ttf", Game.SpriteBatch);

            Texture2D _bg = new Texture2D("Content/Textures/Images/Menu/spacebox-blurred.jpg");
            Game.UiManager.Add("bg",
                new Cubic.GUI.Image(Game.UiManager, _bg,
                    new Position(DockType.Center, -new Vector2(_bg.Width, _bg.Height) / 2 + new Vector2(300, 50)), Vector2.One, Color.White));

            const int gap = 10;
            
            Button continueButton = new Button(Game.UiManager, new Position(DockType.BottomLeft, new Vector2(50, -470)),
                new Size(232, 100), "Continue", fontSize: 48);
            //continueButton.OnClick += () => Console.WriteLine("click");

            Button newGameButton = new Button(Game.UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(continueButton.Position.Offset.X,
                        continueButton.Position.Offset.Y + continueButton.Size.Height + gap)), new Size(232, 50),
                "New Game");
            
            Button loadGameButton = new Button(Game.UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(newGameButton.Position.Offset.X,
                        newGameButton.Position.Offset.Y + newGameButton.Size.Height + gap)), new Size(232, 50),
                "Load Game");
            
            Button multiplayerButton = new Button(Game.UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(loadGameButton.Position.Offset.X,
                        loadGameButton.Position.Offset.Y + loadGameButton.Size.Height + gap)), new Size(232, 50),
                "Multiplayer");
            
            Button settingsButton = new Button(Game.UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(multiplayerButton.Position.Offset.X,
                        multiplayerButton.Position.Offset.Y + multiplayerButton.Size.Height + gap)), new Size(232, 50),
                "Settings");
            
            Button quitButton = new Button(Game.UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(settingsButton.Position.Offset.X,
                        settingsButton.Position.Offset.Y + settingsButton.Size.Height + gap)), new Size(232, 50),
                "Quit");
            quitButton.OnClick += () => Game.Close();
            
            Game.UiManager.Add("continueButton", continueButton);
            Game.UiManager.Add("newGameButton", newGameButton);
            Game.UiManager.Add("loadGameButton", loadGameButton);
            Game.UiManager.Add("multiplayerButton", multiplayerButton);
            Game.UiManager.Add("settingsButton", settingsButton);
            Game.UiManager.Add("quitButton", quitButton);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            
            Game.UiManager.Draw();

            _font.DrawString(128, "space", new Vector2(50, 50), Vector2.One, Color.White);
            _font.DrawString(128, "box", new Vector2(50, 130), Vector2.One, Color.White);
        }
    }
}