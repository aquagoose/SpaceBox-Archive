using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cubic.Data;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using Cubic.Windowing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Spacebox.Game.Scenes
{
    public class IntroScene : Scene
    {
        private Texture2D _currentLogo;
        
        private Texture2D _ismLogo;
        private Texture2D _spaceboxLogo;

        private Texture2D _load;

        private float _alpha;
        private float _rotAlpha;
        private Color _color;

        private float _rot;

        private float _startTime;
        private float _endTime = -1;

        private bool _hasLoaded;
        private bool _hasGotFiles;
        private bool _filesLoaded;

        private Dictionary<string, Bitmap[]> _loaded;

        public override void Initialize(SpaceboxGame game)
        {
            base.Draw(game);
            
            GL.ClearColor(Color.Black);
            
            Console.WriteLine("LOADING CTF... (This may take a while!");

            _ismLogo = new Texture2D("Content/Textures/Images/ismlogo.ctf", autoDispose: false);
            _spaceboxLogo = new Texture2D("Content/Textures/Images/spaceboxlogo.ctf", autoDispose: false);
            
            _load = new Texture2D("Content/Textures/Images/loading2.ctf", autoDispose: false);
            
            UiManager.Add("loading", new Label(UiManager, new Position(DockType.BottomRight, new Vector2(-435, -75)), "Loading, please wait...")
            {
                Visible = false
            });

            _currentLogo = _ismLogo;

            _startTime = Time.ElapsedSeconds;
            _alpha = 0;

            _loaded = new Dictionary<string, Bitmap[]>();
        }

        public override void Update(SpaceboxGame game)
        {
            base.Update(game);

            Input.CursorState = CursorState.Captured;
            
            // The amount of time it takes for the image to fade, in seconds.
            const float fadeTime = 0.5f;
            // Times[0] is the time until the image starts to fade in after scene has loaded
            // Times[1] is the time the image will display for until fade out
            // Times[2] is the time the scene will change after the image has faded out.
            int[] times = { 1, 3, 2 };

            if (Time.ElapsedSeconds - _startTime - times[0] - times[1] > times[2])
            {
                if (_currentLogo == _ismLogo)
                {
                    _currentLogo = _spaceboxLogo;
                    _startTime = Time.ElapsedSeconds;
                }
                else if (!_hasLoaded)
                {
                    _rotAlpha = 1;
                    UiManager.GetElement<Label>("loading").Visible = true;

                    if (!_hasGotFiles)
                    {
                        Console.WriteLine("Loading!");
                        _hasGotFiles = true;
                        
                        LoadTextures();
                    }

                    if (_endTime < 0)
                        _endTime = Time.ElapsedSeconds;
                    if (_filesLoaded)
                    {
                        Console.WriteLine("Finalising...");
                        foreach (KeyValuePair<string, Bitmap[]> bp in _loaded)
                        {
                            Console.WriteLine($"Setting {bp.Key}...");
                            Content.LoadedTextures.Add(bp.Key, new Texture2D(bp.Value[0]));
                        }
                    
                        Console.WriteLine("Loading done!");
                        _hasLoaded = true;
                        _rotAlpha = 0;
                        UiManager.GetElement<Label>("loading").Visible = false;
                        _startTime = Time.ElapsedSeconds - (times[0] + times[1]);
                    }
                }
                else
                    SceneManager.SetScene(new MenuScene());
            }
            else if (Time.ElapsedSeconds - _startTime - times[0] > times[1])
            {
                if (_currentLogo != _spaceboxLogo || _hasLoaded)
                    _alpha = MathHelper.Lerp(1, 0, (Time.ElapsedSeconds - _startTime - times[0] - times[1]) / fadeTime);
            }
            else if (Time.ElapsedSeconds - _startTime > times[0])
                _alpha = MathHelper.Lerp(0, 1, (Time.ElapsedSeconds - _startTime - times[0]) / fadeTime);

            _color = Color.FromArgb((int) (_alpha * 255f), Color.White);

            _rot += 5 * Time.DeltaTime;
        }

        public override void Draw(SpaceboxGame game)
        {
            base.Draw(game);
            
            SpriteBatch.Begin();

            Vector2 imgScale = new Vector2(SpriteBatch.Width / (float) _currentLogo.Width,
                SpriteBatch.Height / (float) _currentLogo.Height);
            SpriteBatch.Draw(_currentLogo, new Vector2(SpriteBatch.Width, SpriteBatch.Height) / 2f, _color,
                0, _currentLogo.Size.ToVector2() / 2f, new Vector2(imgScale.X < imgScale.Y ? imgScale.X : imgScale.Y));

            Vector2 scale = new Vector2(1 / 8f);
            SpriteBatch.Draw(_load,
                new Vector2(SpriteBatch.Width - _load.Width * scale.X * UiManager.UiScale.X,
                    SpriteBatch.Height - _load.Height * scale.Y * UiManager.UiScale.Y),
                Color.FromArgb((int) (_rotAlpha * 255f), Color.White), _rot, new Vector2(_load.Width, _load.Height) / 2,
                scale * UiManager.UiScale);
            
            SpriteBatch.End();
            
            UiManager.Draw();
        }

        public override void Unload()
        {
            base.Unload();
            
            _ismLogo.Dispose();
            _spaceboxLogo.Dispose();
        }

        public async Task LoadTextures()
        {
            string[] files = Directory.GetFiles("Content/Textures", "*.ctf", SearchOption.AllDirectories);
            _hasGotFiles = true;
            foreach (string file in files)
            {
                Console.WriteLine($"Loading {file}...");
                string key = Path.GetFileNameWithoutExtension(file);
                Bitmap[] bp = await Task.Run(() => Texture2D.LoadCTF(file));
                _loaded.Add(key, bp);
            }

            _filesLoaded = true;
        }
    }
}