using System;
using System.Collections.Generic;
using System.Drawing;
using Cubic.Render;
using FreeTypeSharp;
using FreeTypeSharp.Native;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Cubic.GUI.Fonts
{
    public class Font : IDisposable
    {
        private FreeTypeLibrary _library;
        private uint _storedFontSize;
        private Dictionary<char, Character> _characters;
        private IntPtr _facePtr;

        private int _vao;
        private int _vbo;

        private Shader _shader;
        private SpriteBatch _batch;

        public Font(string path, SpriteBatch batch)
        {
            _shader = new Shader("Content/Shaders/2D/Font.vert", "Content/Shaders/2D/Font.frag");
            _characters = new Dictionary<char, Character>();
            _storedFontSize = 0;
            _batch = batch;
            _batch.Resized += BatchOnResize;
            _library = new FreeTypeLibrary();
            FT.FT_New_Face(_library.Native, path, 0, out _facePtr);
        }

        private void BatchOnResize()
        {
            _shader.Use();
            _shader.SetUniform("uProjection",
                Matrix4.CreateOrthographicOffCenter(0f, _batch.Width, _batch.Height, 0f, -1f, 1f));
        }

        // Ultimately in the end I don't want to be doing it like this. It's memory intensive, and wastes a lot of GPU
        // power. It's a good start, but I don't like the result.
        private void GetFont()
        {
            int currentUnpackAlignment = GL.GetInteger(GetPName.UnpackAlignment);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            
            // Delete the textures from memory as we won't need them anymore.
            foreach (KeyValuePair<char, Character> c in _characters)
                GL.DeleteTexture(c.Value.TexId);
            _characters.Clear();

            FT.FT_Set_Pixel_Sizes(_facePtr, 0, _storedFontSize);
            FreeTypeFaceFacade face = new FreeTypeFaceFacade(_library, _facePtr);

            // C++. LOL
            for (uint c = 0; c < 128; c++)
            {
                FT.FT_Load_Char(_facePtr, c, FT.FT_LOAD_RENDER);
                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, (int)face.GlyphBitmap.width,
                    (int)face.GlyphBitmap.rows, 0, PixelFormat.Red, PixelType.UnsignedByte, face.GlyphBitmap.buffer);
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMinFilter.Linear);

                Character chr = new Character()
                {
                    TexId = texture,
                    Size = new Vector2(face.GlyphBitmap.width, face.GlyphBitmap.rows),
                    Bearing = new Vector2(face.GlyphBitmapLeft, face.GlyphBitmapTop),
                    Advance = face.GlyphMetricHorizontalAdvance
                };
                
                _characters.Add((char) c, chr);
            }
            
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            GL.BufferData(BufferTarget.ArrayBuffer, 6 * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            
            _shader.Use();
            _shader.SetUniform("uProjection",
                Matrix4.CreateOrthographicOffCenter(0f, _batch.Width, _batch.Height, 0f, -1f, 1f));

            int vLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vLocation);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            
            if (currentUnpackAlignment != 1)
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, currentUnpackAlignment);
        }

        public void DrawString(uint fontSize, string text, Vector2 position, Vector2 scale, Color color)
        {
            if (fontSize != _storedFontSize)
            {
                _storedFontSize = fontSize;
                GetFont();
            }

            bool isBlendCapEnabled = GL.IsEnabled(EnableCap.Blend);
            int currentBlendSrc = GL.GetInteger(GetPName.BlendSrc);
            int currentBlendDst = GL.GetInteger(GetPName.BlendDst);
            bool isCullFaceEnabled = GL.IsEnabled(EnableCap.CullFace);
            
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha); 
            GL.Disable(EnableCap.CullFace);
            
            _shader.Use();
            _shader.SetUniform("uColor", color);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(_vao);
            
            Vector2 largestChar = Vector2.Zero;
            foreach (char c in text)
            {
                Character ch = _characters[c];
                if (ch.Bearing.Y > largestChar.Y)
                    largestChar = ch.Size;
            }

            position.Y += largestChar.Y;
            
            foreach (char c in text)
            {
                Character ch = _characters[c];

                Vector2 pos = new Vector2(position.X + ch.Bearing.X * scale.X,
                    position.Y - ch.Size.Y + (ch.Size.Y - ch.Bearing.Y) * scale.Y);

                Vector2 wh = ch.Size * scale;

                float[,] vertices = new float[,]
                {
                    { pos.X, pos.Y + wh.Y, 0.0f, 1.0f },
                    { pos.X, pos.Y, 0.0f, 0.0f },
                    { pos.X + wh.X, pos.Y, 1.0f, 0.0f },
                    { pos.X, pos.Y + wh.Y, 0.0f, 1.0f },
                    { pos.X + wh.X, pos.Y, 1.0f, 0.0f },
                    { pos.X + wh.X, pos.Y + wh.Y, 1.0f, 1.0f }
                    
                    /*{ 0, wh.Y, 0.0f, 1.0f },
                    { 0, 0, 0.0f, 0.0f },
                    { wh.X, 0, 1.0f, 0.0f },
                    { 0, wh.Y, 0.0f, 1.0f },
                    { wh.X, 0, 1.0f, 0.0f },
                    { wh.X, wh.Y, 1.0f, 1.0f }*/
                };
                
                GL.BindTexture(TextureTarget.Texture2D, ch.TexId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                position.X += ch.Advance * scale.X;
            }
            
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            
            if (isCullFaceEnabled)
                GL.Enable(EnableCap.CullFace);
            if (isBlendCapEnabled)
                GL.Enable(EnableCap.Blend);
            GL.BlendFunc((BlendingFactor) currentBlendSrc, (BlendingFactor) currentBlendDst);
        }

        public Vector2 MeasureString(uint fontSize, string text)
        {
            if (fontSize != _storedFontSize)
            {
                _storedFontSize = fontSize;
                GetFont();
            }
            
            Vector2 size = Vector2.Zero;

            foreach (char c in text)
            {
                Character ch = _characters[c];

                if (ch.Bearing.Y > size.Y)
                    size.Y = ch.Bearing.Y;
                size.X += ch.Advance;
            }

            return size;
        }
        
        public void Dispose()
        {
            FT.FT_Done_Face(_facePtr);
            _library.Dispose();
            GC.SuppressFinalize(this);
        }

        private struct Character
        {
            public int TexId;
            public Vector2 Size;
            public Vector2 Bearing;
            public int Advance;
        }
    }
}