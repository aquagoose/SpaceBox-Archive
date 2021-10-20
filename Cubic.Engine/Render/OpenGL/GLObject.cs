using System;

namespace Cubic.Engine.Render.OpenGL
{
    public abstract class GLObject : IDisposable
    {
        /// <summary>
        /// The raw OpenGL handle for this GL Object.
        /// </summary>
        public readonly int Handle;

        protected GLObject(int handle)
        {
            Handle = handle;
        }

        public abstract void Dispose();
    }
}