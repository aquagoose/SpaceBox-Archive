using System.Collections.Generic;
using System.Drawing;
using Cubic.Data;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Windowing;
using OpenTK.Graphics.OpenGL4;
using SpaceBox.Data;
using SpaceBox.GUI;
using SpaceBox.GUI.Imgui;
using SpaceBox.Sandbox.Grids;
using Vector2 = OpenTK.Mathematics.Vector2;
using Font = Cubic.GUI.Fonts.Font;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Spacebox.Game.Scenes
{
    public class MenuScene : Scene
    {
        private string[] _resolutions;
        private int _currentRes;

        private MenuImageSystem _system;

        private SettingsWindow _window;
        private NewGameWindow _newGameWindow;
        private LoadGameWindow _loadGameWindow;

        private Font _font;

        public override void Initialize(SpaceboxGame game)
        {
            base.Initialize(game);
            
            GL.Disable(EnableCap.DepthTest);
            Input.CursorState = CursorState.Visible;

            GL.ClearColor(Color.FromArgb(178, 178, 178));

            _font = new Font("Content/Fonts/inversionz.ttf", SpriteBatch);

            Texture2D _bg = new Texture2D(Content.LoadedTextures["spacebox-blurred"][0]);
            UiManager.Add("bg",
                new Cubic.GUI.Image(UiManager, _bg,
                    new Position(DockType.Center, -new Vector2(_bg.Width, _bg.Height) / 2 + new Vector2(300, 50)), Vector2.One, Color.White));

            const int gap = 10;
            
            Button continueButton = new Button(UiManager, new Position(DockType.BottomLeft, new Vector2(50, -470)),
                new Size(232, 100), "Continue", fontSize: 48);
            continueButton.OnClick += () => SceneManager.SetScene(new MainScene());

            Button newGameButton = new Button(UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(continueButton.Position.Offset.X,
                        continueButton.Position.Offset.Y + continueButton.Size.Height + gap)), new Size(232, 50),
                "New Game");
            newGameButton.OnClick += () => _newGameWindow.ShouldShow = true;
            
            Button loadGameButton = new Button(UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(newGameButton.Position.Offset.X,
                        newGameButton.Position.Offset.Y + newGameButton.Size.Height + gap)), new Size(232, 50),
                "Load Game");
            loadGameButton.OnClick += () => _loadGameWindow.ShouldShow = true;
            
            Button multiplayerButton = new Button(UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(loadGameButton.Position.Offset.X,
                        loadGameButton.Position.Offset.Y + loadGameButton.Size.Height + gap)), new Size(232, 50),
                "Multiplayer");
            
            Button settingsButton = new Button(UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(multiplayerButton.Position.Offset.X,
                        multiplayerButton.Position.Offset.Y + multiplayerButton.Size.Height + gap)), new Size(232, 50),
                "Settings");
            settingsButton.OnClick += () => _window.ShouldShow = true;
            
            Button quitButton = new Button(UiManager,
                new Position(DockType.BottomLeft,
                    new Vector2(settingsButton.Position.Offset.X,
                        settingsButton.Position.Offset.Y + settingsButton.Size.Height + gap)), new Size(232, 50),
                "Quit");
            quitButton.OnClick += SpaceboxGame.Exit;
            
            UiManager.Add("continueButton", continueButton);
            UiManager.Add("newGameButton", newGameButton);
            UiManager.Add("loadGameButton", loadGameButton);
            UiManager.Add("multiplayerButton", multiplayerButton);
            UiManager.Add("settingsButton", settingsButton);
            UiManager.Add("quitButton", quitButton);

            UiManager.Add("disable",
                new FillRectangle(UiManager, continueButton.Position, continueButton.Size,
                    Color.FromArgb(72, Color.Black)));
            UiManager.Add("disable2",
                new FillRectangle(UiManager, multiplayerButton.Position, multiplayerButton.Size,
                    Color.FromArgb(72, Color.Black)));

            UiManager.Add("spaceText",
                new Label(UiManager, new Position(50, 50), "space", "Content/Fonts/inversionz.ttf", 128));
            UiManager.Add("boxText",
                new Label(UiManager, new Position(50, 130), "box", "Content/Fonts/inversionz.ttf", 128));

            UiManager.Add("fillRect",
                new FillRectangle(UiManager, new Position(0, 0), Size.Empty, Color.FromArgb(128, 0, 0, 0))
                {
                    Visible = false,
                    IgnoreScale = true
                });

            _window = new SettingsWindow(SpaceboxGame.Config, game);
            _newGameWindow = new NewGameWindow();
            _loadGameWindow = new LoadGameWindow();
        }

        public override void Update(SpaceboxGame game)
        {
            base.Update(game);

            FillRectangle fillRect = UiManager.GetElement<FillRectangle>("fillRect");
            
            fillRect.Size =
                new Size(SpriteBatch.Width, SpriteBatch.Height);

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
                    Data.SaveWorld(_newGameWindow.WorldName, Vector3.Zero, Quaternion.Identity, new List<Grid>());
                    SceneManager.SetScene(new MainScene(_newGameWindow.WorldName));
                }
            }

            if (_loadGameWindow.ShouldShow)
            {
                if (_loadGameWindow.Display())
                {
                    SaveGame saveGame = Data.LoadSave(_loadGameWindow.WorldFiles[_loadGameWindow.SelectedWorld]);
                    SceneManager.SetScene(new MainScene(save: saveGame));
                }
            }
        }

        public override void Draw(SpaceboxGame game)
        {
            base.Draw(game);
            
            UiManager.Draw();

            //_font.DrawString(128, "space", new Vector2(50, 50), Vector2.One, Color.White);
            //_font.DrawString(128, "box", new Vector2(50, 130), Vector2.One, Color.White);
        }
    }
}