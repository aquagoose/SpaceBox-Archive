using System.Collections.Generic;
using Cubic.Engine.Render.OpenGL;
using OpenTK.Graphics.OpenGL4;

namespace Cubic.Engine.Render
{
    public class ForwardRenderer
    {
        private Dictionary<string, Shader> _shaders;
        //private 

        public ForwardRenderer()
        {
            _shaders = new Dictionary<string, Shader>();
        }

        public void DrawVertexArray(VertexArray array)
        {
            GL.DrawElements(PrimitiveType.Triangles, );
        }
    }
}