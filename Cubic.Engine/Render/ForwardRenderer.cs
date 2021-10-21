using System.Collections.Generic;
using Cubic.Engine.Render.OpenGL;
using Cubic.Engine.Render.Shaders;
using Cubic.Engine.Utilities;
using OpenTK.Graphics.OpenGL4;

namespace Cubic.Engine.Render
{
    public class ForwardRenderer
    {
        private Dictionary<string, Shader> _shaders;

        private Camera _camera;
        
        public ForwardRenderer()
        {
            _shaders = new Dictionary<string, Shader>();
        }

        public void SetCamera(Camera camera)
        {
            _camera = camera;
        }

        public void DrawVertexArray(VertexArray array, Texture2D texture, Shader shader)
        {
            texture.Bind();
            array.Bind();
            shader.Use();
            
            
            
            if (array.IndexBufferDataLength > 0)
                GL.DrawElements(PrimitiveType.Triangles, array.IndexBufferDataLength, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(PrimitiveType.Triangles, 0, array.VertexBufferDataLength);
            array.Unbind();
        }
    }
}