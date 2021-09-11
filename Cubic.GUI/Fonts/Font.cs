using System;
using FreeTypeSharp.Native;
using OpenTK.Mathematics;

namespace Cubic.GUI.Fonts
{
    public class Font : IDisposable
    {
        public Font()
        {
            
        }
        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private struct Character
        {
            public int TextureID;
            public Vector2 Size;
            public Vector2 Bearing;
            public int Advance;
        }
    }
}