using System;
using OpenTK.Graphics.OpenGL4;

namespace Cubic.Engine.Render.OpenGL
{
    public class VertexArray : GLObject
    {
        public VertexArray() : base(GL.GenVertexArray())
        {
        }

        public void SetBuffer<T>(Buffer<T> buffer) where T : struct
        {
            Bind();
            buffer.Bind();
            Unbind();
            buffer.Unbind();
        }

        internal void Bind()
        {
            GL.BindVertexArray(Handle);
        }

        internal void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public override void Dispose()
        {
            Unbind();
            GL.DeleteVertexArray(Handle);
            GC.SuppressFinalize(this);
        }
    }
}