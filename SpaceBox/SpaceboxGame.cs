using System.Drawing;
using Cubic.Render;
using Cubic.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Spacebox.Scenes;

namespace Spacebox
{
    public class SpaceboxGame : GameWindow
    {
        public SpriteRenderer SpriteBatch;

        private Scene _activeScene;

        public SpaceboxGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad()
        {
            base.OnLoad();
            
            GL.ClearColor(Color.Black);

            SpriteBatch = new SpriteRenderer(this);

            _activeScene = new IntroScene(this);
            _activeScene.Initialize();
            
            // Only show the window once initialization has completed.
            CenterWindow();
            IsVisible = true;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            _activeScene.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _activeScene.Draw();
            
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            if (disposing)
            {
                _activeScene.Dispose();
                SpriteBatch.Dispose();
                DisposeManager.DisposeAll();
            }
        }
    }
}