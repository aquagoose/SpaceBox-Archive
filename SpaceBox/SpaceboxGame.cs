using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Reflection;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceBox.Data;
using SpaceBox.GUI.Imgui;
using Spacebox.Scenes;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Spacebox
{
    public class SpaceboxGame : GameWindow
    {
        private bool _disposed;
        
        public SpriteBatch SpriteBatch;

        public static SpaceboxConfig Config;

        public UIManager UiManager;

        private Scene _activeScene;
        private Scene _transitionScene;

        private ImGuiRenderer _imGuiRenderer;
        public ImFontPtr Arial;

        private NativeWindowSettings _nativeWindowSettings;

        public SpaceboxGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings,
            SpaceboxConfig config) : base(gameWindowSettings, nativeWindowSettings)
        {
            Config = config;
            _nativeWindowSettings = nativeWindowSettings;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            
            Time.Start();
            
            GL.ClearColor(Color.Black);

            SpriteBatch = new SpriteBatch(this);
            UiManager = new UIManager(SpriteBatch);
            
            _imGuiRenderer = new ImGuiRenderer(this);
            
            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiConfig.Fonts.Add("arial", io.Fonts.AddFontFromFileTTF("Content/Fonts/daggersquare.otf", 20));
            _imGuiRenderer.RecreateFontDeviceTexture();
            
            if (WindowState == WindowState.Fullscreen)
                Size = _nativeWindowSettings.Size;
            
            // Only show the window once initialization has completed.
            if (WindowState != WindowState.Fullscreen)
                CenterWindow();
            
            //_activeScene = new IntroScene(this);
            _activeScene = new MenuScene(this);
            _activeScene.Initialize();

            Console.WriteLine(WindowState);
            
            IsVisible = true;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            Time.Update();

            Input.Update(KeyboardState, MouseState);
            
            _imGuiRenderer.Update(Time.DeltaTime);

            _activeScene.Update();

            UiManager.Update();
            
            if (Input.IsKeyPressed(Config.Input.TakeScreenshot))
            {
                Bitmap bitmap = new Bitmap(ClientSize.X, ClientSize.Y);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                
                GL.ReadPixels(0, 0, ClientSize.X, ClientSize.Y, PixelFormat.Bgra, PixelType.UnsignedByte,
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

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            _activeScene.Draw();

            _imGuiRenderer.Render();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            
            GL.Viewport(0, 0, e.Width, e.Height);

            Size winSize = new Size(UiManager.SpriteBatch.Width, UiManager.SpriteBatch.Height);
            float refSize = winSize.Width > winSize.Height ? winSize.Height : winSize.Width;

            _imGuiRenderer.ScaleFactor = new System.Numerics.Vector2(refSize / (winSize.Width > winSize.Height
                ? UiManager.ReferenceResolution.Height
                : UiManager.ReferenceResolution.Width));

            //Console.WriteLine("Resize");
            //OnRenderFrame(new FrameEventArgs(Time.DeltaTime));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_disposed)
                return;
            
            if (disposing)
            {
                _activeScene.Dispose();
                UiManager.Clear();
                SpriteBatch.Dispose();
                _imGuiRenderer.Dispose();
                DisposeManager.DisposeAll();
            }

            _disposed = true;
        }

        public void SetScene(Scene scene)
        {
            _transitionScene = scene;
        }
    }
}