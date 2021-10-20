using System;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Cubic.Engine.Render.OpenGL
{
    public class Buffer<T> : GLObject where T : struct
    {
        internal readonly BufferTarget BufferTarget;

        public Buffer(BufferType bufferType) : base(GL.GenBuffer())
        {
            BufferTarget = bufferType switch
            {
                BufferType.VertexBuffer => BufferTarget.ArrayBuffer,
                BufferType.IndexBuffer => BufferTarget.ElementArrayBuffer,
                _ => throw new ArgumentOutOfRangeException(nameof(bufferType), bufferType, null)
            };
        }

        public void SetData(T[] data)
        {
            GL.BindBuffer(BufferTarget, Handle);
            GL.BufferData(BufferTarget, data.Length * Unsafe.SizeOf<T>(), data, BufferUsageHint.StaticDraw);
            
            GL.BindBuffer(BufferTarget, 0);
        }

        internal void Bind()
        {
            GL.BindBuffer(BufferTarget, Handle);
        }

        internal void Unbind()
        {
            GL.BindBuffer(BufferTarget, 0);
        }
        
        public override void Dispose()
        {
            Unbind();
            GL.DeleteBuffer(Handle);
            GC.SuppressFinalize(this);
        }
    }

    public enum BufferType
    {
        VertexBuffer,
        IndexBuffer
    }
}