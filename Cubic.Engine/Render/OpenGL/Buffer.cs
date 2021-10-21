using System;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Cubic.Engine.Render.OpenGL
{
    public class Buffer<T> : GLObject where T : struct
    {
        private BufferTarget _currentTarget;
        internal int DataLength;

        public Buffer(/*BufferType bufferType*/) : base(GL.GenBuffer())
        {
            /*BufferTarget = bufferType switch
            {
                BufferType.VertexBuffer => BufferTarget.ArrayBuffer,
                BufferType.IndexBuffer => BufferTarget.ElementArrayBuffer,
                _ => throw new ArgumentOutOfRangeException(nameof(bufferType), bufferType, null)
            };*/
        }

        public void SetData(T[] data)
        {
            //GL.BindBuffer(BufferTarget, Handle);
            //GL.BufferData(BufferTarget, data.Length * Unsafe.SizeOf<T>(), data, BufferUsageHint.StaticDraw);
            
            //GL.BindBuffer(BufferTarget, 0);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Unsafe.SizeOf<T>(), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            DataLength = data.Length;
        }

        internal void Bind(BufferTarget target)
        {
            //GL.BindBuffer(BufferTarget, Handle);
            GL.BindBuffer(target, Handle);
            _currentTarget = target;
        }

        internal void Unbind()
        {
            GL.BindBuffer(_currentTarget, 0);
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