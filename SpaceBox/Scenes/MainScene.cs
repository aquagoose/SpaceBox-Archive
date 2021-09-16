using System.Drawing;
using OpenTK.Graphics.OpenGL4;

namespace Spacebox.Scenes
{
    public class MainScene : Scene
    {
        public MainScene(SpaceboxGame game) : base(game) { }

        public override void Initialize()
        {
            base.Initialize();
            
            GL.ClearColor(Color.Black);
            
            
        }
    }
}