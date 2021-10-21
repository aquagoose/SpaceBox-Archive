using System;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace Cubic.Engine.Render.OpenGL
{
    public class VertexArray : GLObject
    {
        internal int VertexBufferDataLength;
        internal int IndexBufferDataLength;
        
        public VertexArray() : base(GL.GenVertexArray())
        {
        }

        public void SetVertexBuffer(Buffer<float> vertex)
        {
            Bind();
            vertex.Bind(BufferTarget.ArrayBuffer);
            Unbind();
            vertex.Unbind();
            VertexBufferDataLength = vertex.DataLength;
        }

        public void SetIndexBuffer(Buffer<uint> index)
        {
            Bind();
            index.Bind(BufferTarget.ElementArrayBuffer);
            Unbind();
            index.Unbind();
            IndexBufferDataLength = index.DataLength;
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