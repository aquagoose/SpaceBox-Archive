using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using Cubic.Windowing;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using SpaceBox.Data;
using SpaceBox.GUI.Imgui;
using Spacebox.Scenes;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Spacebox
{
    public class SpaceboxGame : BaseGame
    {
        private bool _disposed;
        
        public SpriteBatch SpriteBatch;

        public static SpaceboxConfig Config;

        public UIManager UiManager;

        private Scene _activeScene;
        private Scene _transitionScene;

        private ImGuiRenderer _imGuiRenderer;

        public SpaceboxGame(WindowSettings settings, SpaceboxConfig config) : base(settings)
        {
            Config = config;
        }

        protected override void Initialize()
        {
            base.Initialize();

            GL.ClearColor(Color.Black);

            SpriteBatch = new SpriteBatch(this);
            UiManager = new UIManager(SpriteBatch);
            
            _imGuiRenderer = new ImGuiRenderer(this);
            
            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiConfig.Fonts.Add("arial", io.Fonts.AddFontFromFileTTF("Content/Fonts/daggersquare.otf", 20));
            _imGuiRenderer.RecreateFontDeviceTexture();

            //_activeScene = new IntroScene(this);
            _activeScene = new MenuScene(this);
            //_activeScene = new MainScene(this);
            _activeScene.Initialize();
            
            Resize += WindowResize;
        }

        protected override void Update()
        {
            base.Update();

            _imGuiRenderer.Update(Time.DeltaTime);

            _activeScene.Update();

            UiManager.Update();
            
            if (Input.IsKeyPressed(Config.Input.TakeScreenshot))
            {
                Bitmap bitmap = new Bitmap(Size.Width, Size.Height);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                
                GL.ReadPixels(0, 0, Size.Width, Size.Height, PixelFormat.Bgra, PixelType.UnsignedByte,
                    data.Scan0);
                
                bitmap.UnlockBits(data);
                
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                DateTime now = DateTime.Now;

                if (!Directory.Exists(Path.Combine(Data.SpaceBoxFolderLocation, Data.SpaceBoxFolderName,
                    "Screenshots")))
                    Directory.CreateDirectory(Path.Combine(Data.SpaceBoxFolderLocation, Data.SpaceBoxFolderName,
                        "Screenshots"));
                
                bitmap.Save(Path.Combine(Data.SpaceBoxFolderLocation, Data.SpaceBoxFolderName, "Screenshots",
                    $"Screenshot_{now.Year}{now.Month}{now.Day}_{now.Hour}{now.Minute}{now.Second}.jpg"));
                bitmap.Dispose();

                Console.WriteLine("Saved screenshot.");
            }

            if (_transitionScene != null)
            {
                Console.WriteLine("Transition");
                _activeScene.Dispose();
                UiManager.Clear();
                _activeScene = _transitionScene;
                _transitionScene = null;
                _activeScene.Initialize();
            }
        }

        protected override void Draw()
        {
            base.Draw();
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            _activeScene.Draw();

            _imGuiRenderer.Render();
        }

        private void WindowResize(Size size)
        {
            GL.Viewport(0, 0, size.Width, size.Height);

            Size winSize = new Size(UiManager.SpriteBatch.Width, UiManager.SpriteBatch.Height);
            float refSize = winSize.Width > winSize.Height ? winSize.Height : winSize.Width;

            _imGuiRenderer.ScaleFactor = new System.Numerics.Vector2(refSize / (winSize.Width > winSize.Height
                ? UiManager.ReferenceResolution.Height
                : UiManager.ReferenceResolution.Width));
            
            Console.WriteLine(size);

            //Console.WriteLine("Resize");
            //OnRenderFrame(new FrameEventArgs(Time.DeltaTime));
        }

        protected override void Unload()
        {
            base.Unload();
            
            _activeScene.Dispose();
            UiManager.Clear();
            SpriteBatch.Dispose();
            _imGuiRenderer.Dispose();
            DisposeManager.DisposeAll();
        }

        public void SetScene(Scene scene)
        {
            _transitionScene = scene;
        }
    }
}