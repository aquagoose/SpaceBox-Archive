using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using Cubic.Data;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using Cubic.Windowing;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using SpaceBox.Data;
using Spacebox.Game.Scenes;
using SpaceBox.GUI.Imgui;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Spacebox.Game
{
    public class SpaceboxGame : BaseGame
    {
        private bool _disposed;
        
        public SpriteBatch SpriteBatch;

        public static SpaceboxConfig Config;

        public UIManager UiManager;

        private ImGuiRenderer _imGuiRenderer;

        private static bool _shouldClose;

        //private OpenGLDebugger _debugger;

        public SpaceboxGame(WindowSettings settings, SpaceboxConfig config) : base(settings)
        {
            Config = config;
        }

        protected override void Initialize()
        {
            base.Initialize();

            //_debugger = new OpenGLDebugger();

            GL.ClearColor(Color.Black);

            SpriteBatch = new SpriteBatch(this);
            UiManager = new UIManager(SpriteBatch);
            
            _imGuiRenderer = new ImGuiRenderer(this);
            
            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiConfig.Fonts.Add("arial", io.Fonts.AddFontFromFileTTF("Content/Fonts/daggersquare.otf", 20));
            ImGuiConfig.Fonts.Add("large", io.Fonts.AddFontFromFileTTF("Content/Fonts/daggersquare.otf", 35));
            _imGuiRenderer.RecreateFontDeviceTexture();

            //_activeScene = new IntroScene(this);
#if DEBUG
            Content.LoadAllTextures("Content/Textures");
            while (!Content.Loaded)
                continue;
            SceneManager.Initialize(this, new MenuScene(), SpriteBatch, UiManager);
#else
            SceneManager.Initialize(this, new IntroScene(), SpriteBatch, UiManager);
#endif

            Resize += WindowResize;
        }

        protected override void Update()
        {
            base.Update();

            _imGuiRenderer.Update(Time.DeltaTime);

            SceneManager.Update(this);

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

            if (_shouldClose)
                Close();
        }

        protected override void Draw()
        {
            base.Draw();
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            SceneManager.Draw(this);

#if !DEBUG
            // Using ImGui here because I don't want to add 300 draw calls to the game which already suffers from enough
            // of them.
            ImGui.PushFont(ImGuiConfig.Fonts["large"]);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, ImGui.GetColorU32(new Vector4(0, 0, 0, 0.5f)));
            if (ImGui.Begin("E",
                ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings))
            {
                ImGui.Text("SpaceBox Alpha\nDEVELOPMENT BUILD - NOT FINAL");
                ImGui.SetWindowPos(new Vector2((Size.Width * 1 / UiManager.UiScale.X) - ImGui.GetWindowWidth(), 0));
                ImGui.End();
            }
            ImGui.PopFont();
            ImGui.PopStyleColor();

#endif

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
            
            //_activeScene.Dispose();
            SceneManager.DisposeScenes();
            UiManager.Clear();
            SpriteBatch.Dispose();
            _imGuiRenderer.Dispose();
            DisposeManager.DisposeAll();
        }

        public static void Exit()
        {
            _shouldClose = true;
        }
    }
}