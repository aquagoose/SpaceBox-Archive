using System;
using System.Drawing;
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
using SpaceBox.Data;
using SpaceBox.GUI.Imgui;
using Spacebox.Scenes;

namespace Spacebox
{
    public class SpaceboxGame : GameWindow
    {
        private bool _disposed;
        
        public SpriteBatch SpriteBatch;

        public static SpaceboxConfig Config;

        public UIManager UiManager;

        private Scene _activeScene;

        private ImGuiRenderer _imGuiRenderer;
        public ImFontPtr Arial;

        public SpaceboxGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings,
            SpaceboxConfig config) : base(gameWindowSettings, nativeWindowSettings)
        {
            Config = config;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            
            Time.Start();
            
            GL.ClearColor(Color.Black);

            SpriteBatch = new SpriteBatch(this);
            UiManager = new UIManager(SpriteBatch);
            
            _activeScene = new IntroScene(this);
            //_activeScene = new MenuScene(this);
            _activeScene.Initialize();

            _imGuiRenderer = new ImGuiRenderer(this);

            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiConfig.Fonts.Add("arial", io.Fonts.AddFontFromFileTTF("Content/Fonts/arial.ttf", 20));
            _imGuiRenderer.RecreateFontDeviceTexture();

            // Only show the window once initialization has completed.
            if (WindowState != WindowState.Fullscreen)
                CenterWindow();
            IsVisible = true;

            TryGetCurrentMonitorDpi(out float hDpi, out float vDpi);
            Console.WriteLine(hDpi);
            Console.WriteLine(vDpi);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            Time.Update();

            Input.Update(KeyboardState, MouseState);
            
            _imGuiRenderer.Update(Time.DeltaTime);

            _activeScene.Update();

            UiManager.Update();
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
            
            Console.WriteLine(ImGui.GetIO().DisplaySize);

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
            _activeScene.Dispose();
            UiManager.Clear();
            _activeScene = scene;
            _activeScene.Initialize();
        }
    }
}