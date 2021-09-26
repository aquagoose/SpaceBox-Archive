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
using SpaceBox.Data;
using SpaceBox.GUI;
using SpaceBox.GUI.Imgui;
using Vector2 = OpenTK.Mathematics.Vector2;
using Font = Cubic.GUI.Fonts.Font;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Spacebox.Scenes
{
    public class MenuScene : Scene
    {
        public MenuScene(SpaceboxGame game) : base(game) { }

        private string[] _resolutions;
        private int _currentRes;

        private MenuImageSystem _system;

        private SettingsWindow _window;
        private NewGameWindow _newGameWindow;
        private LoadGameWindow _loadGameWindow;

        private Font _font;

        public override void Initialize()
        {
            base.Initialize();
            
            GL.Disable(EnableCap.DepthTest);
            Game.CursorVisible = true;

            GL.ClearColor(Color.FromArgb(178, 178, 178));

            _font = new Font("Content/Fonts/inversionz.ttf", Game.SpriteBatch);

            Texture2D _bg = new Texture2D("Content/Textures/Images/Menu/spacebox-blurred.jpg");
            Game.UiManager.Add("bg",
                new Cubic.GUI.Image(Game.UiManager, _bg,
                    new Position(DockType.Center, -new Vector2(_bg.Width, _bg.Height) / 2 + new Vector2(300, 50)), Vector2.One, Color.White));

            const int gap = 10;
            
            Button continueButton = new Button(Game.UiManager, new Position(DockType.BottomLeft, new Vector2(50, -470)),
                new Size(232, 100), "Continue", fontSize: 48);
            continueButton.OnClick += () => Game.SetScene(new MainSceneOld(Game, ""));

            Button newGameButton = new Button(Game.UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(continueButton.Position.Offset.X,
                        continueButton.Position.Offset.Y + continueButton.Size.Height + gap)), new Size(232, 50),
                "New Game");
            newGameButton.OnClick += () => _newGameWindow.ShouldShow = true;
            
            Button loadGameButton = new Button(Game.UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(newGameButton.Position.Offset.X,
                        newGameButton.Position.Offset.Y + newGameButton.Size.Height + gap)), new Size(232, 50),
                "Load Game");
            loadGameButton.OnClick += () => _loadGameWindow.ShouldShow = true;
            
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
            settingsButton.OnClick += () => _window.ShouldShow = true;
            
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

            Game.UiManager.Add("disable",
                new FillRectangle(Game.UiManager, continueButton.Position, continueButton.Size,
                    Color.FromArgb(72, Color.Black)));

            Game.UiManager.Add("spaceText",
                new Label(Game.UiManager, new Position(50, 50), "space", "Content/Fonts/inversionz.ttf", 128));
            Game.UiManager.Add("boxText",
                new Label(Game.UiManager, new Position(50, 130), "box", "Content/Fonts/inversionz.ttf", 128));

            Game.UiManager.Add("fillRect",
                new FillRectangle(Game.UiManager, new Position(0, 0), Size.Empty, Color.FromArgb(128, 0, 0, 0))
                {
                    Visible = false,
                    IgnoreScale = true
                });

            _window = new SettingsWindow(SpaceboxGame.Config, Game);
            _newGameWindow = new NewGameWindow();
            _loadGameWindow = new LoadGameWindow();
        }

        public override void Update()
        {
            base.Update();

            FillRectangle fillRect = Game.UiManager.GetElement<FillRectangle>("fillRect");
            
            fillRect.Size =
                new Size(Game.SpriteBatch.Width, Game.SpriteBatch.Height);

            if (_window.ShouldShow || _newGameWindow.ShouldShow || _loadGameWindow.ShouldShow)
                fillRect.Visible = true;
            else
                fillRect.Visible = false;
            
            if (_window.ShouldShow)
                _window.Display();

            if (_newGameWindow.ShouldShow)
            {
                if (_newGameWindow.Display())
                {
                    Data.SaveWorld(_newGameWindow.WorldName, Vector3.Zero, Vector3.Zero, new List<Vector3>());
                    Game.SetScene(new MainSceneOld(Game, _newGameWindow.WorldName));
                }
            }

            if (_loadGameWindow.ShouldShow)
            {
                if (_loadGameWindow.Display())
                {
                    SaveGame game = Data.LoadSave(_loadGameWindow.WorldFiles[_loadGameWindow.SelectedWorld]);
                    Game.SetScene(new MainSceneOld(Game, save: game));
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            
            Game.UiManager.Draw();

            //_font.DrawString(128, "space", new Vector2(50, 50), Vector2.One, Color.White);
            //_font.DrawString(128, "box", new Vector2(50, 130), Vector2.One, Color.White);
        }
    }
}