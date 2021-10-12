using System;
using System.Drawing;
using System.Drawing.Imaging;
using Cubic.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Cubic.Render
{
    /// <summary>
    /// Represents a 2D texture.
    /// </summary>
    public class Texture2D : IDisposable
    {
        private int _handle;

        private bool _disposed;
        
        /// <summary>
        /// The public OpenGL handle for this texture.
        /// </summary>
        internal int Handle => _handle;

        public int Width => Size.Width;
        public int Height => Size.Height;
        
        public Size Size { get; }

        /// <summary>
        /// Create a new texture from the given path.
        /// </summary>
        /// <param name="path">The texture path itself.</param>
        /// <param name="wrap">The wrap type of the texture.</param>
        /// <param name="filter">The texture's desired filter.</param>
        /// <param name="mipmap">Should mipmaps be generated for this texture?</param>
        /// <param name="autoDispose">Should this texture automatically dispose on application exit?</param>
        public Texture2D(string path, TextureWrap wrap = TextureWrap.Repeat,
            TextureFilter filter = TextureFilter.Linear, bool mipmap = true, bool autoDispose = true)
        {
            _handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            using (Bitmap bitmap = new Bitmap(path))
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                Size = bitmap.Size;
            }
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) (wrap == TextureWrap.Repeat ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int) (wrap == TextureWrap.Repeat ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge));

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) (filter == TextureFilter.Linear ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) (filter == TextureFilter.Linear ? TextureMagFilter.Linear : TextureMagFilter.Nearest));
            
            if (mipmap)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            
            if (autoDispose) 
                DisposeManager.Add(this);
        }

        /// <summary>
        /// Create a texture with the given width, height, and IntPtr pixels.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="pixels">An IntPtr to pixels.</param>
        /// <param name="wrap">The wrap type of the texture.</param>
        /// <param name="filter">The texture's desired filter.</param>
        /// <param name="mipmap">Should mipmaps be generated for this texture?</param>
        /// <param name="autoDispose">Should this texture automatically dispose on application exit?</param>
        public Texture2D(int width, int height, IntPtr pixels, TextureWrap wrap = TextureWrap.Repeat,
            TextureFilter filter = TextureFilter.Linear, bool mipmap = true, bool autoDispose = true)
        {
            _handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            Size = new Size(width, height);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) (wrap == TextureWrap.Repeat ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int) (wrap == TextureWrap.Repeat ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge));

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) (filter == TextureFilter.Linear ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) (filter == TextureFilter.Linear ? TextureMagFilter.Linear : TextureMagFilter.Nearest));
            
            if (mipmap)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            
            if (autoDispose) 
                DisposeManager.Add(this);
        }
        
        /// <summary>
        /// Create a texture with the given width, height, and colour array of pixels.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="pixels">A colour array of pixels.</param>
        /// <param name="wrap">The wrap type of the texture.</param>
        /// <param name="filter">The texture's desired filter.</param>
        /// <param name="mipmap">Should mipmaps be generated for this texture?</param>
        /// <param name="autoDispose">Should this texture automatically dispose on application exit?</param>
        public Texture2D(int width, int height, Color[] pixels, TextureWrap wrap = TextureWrap.Repeat,
            TextureFilter filter = TextureFilter.Linear, bool mipmap = true, bool autoDispose = true)
        {
            _handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            float[] floatPixels = new float[pixels.Length * 4];
            int c = 0;
            for (int i = 0; i < floatPixels.Length; i += 4)
            {
                floatPixels[i] = pixels[c].R / 255f;
                floatPixels[i + 1] = pixels[c].G / 255f;
                floatPixels[i + 2] = pixels[c].B / 255f;
                floatPixels[i + 3] = pixels[c].A / 255f;
                c++; // Lol, C++
            }
            
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.Float, floatPixels);

            Size = new Size(width, height);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) (wrap == TextureWrap.Repeat ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int) (wrap == TextureWrap.Repeat ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge));

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) (filter == TextureFilter.Linear ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) (filter == TextureFilter.Linear ? TextureMagFilter.Linear : TextureMagFilter.Nearest));
            
            if (mipmap)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            
            if (autoDispose) 
                DisposeManager.Add(this);
        }

        /// <summary>
        /// Bind this texture to the given texture unit.
        /// </summary>
        /// <param name="unit">The texture unit to bind to (default 0).</param>
        public void Bind(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        /// <summary>
        /// Dispose of the texture.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            GL.DeleteTexture(_handle);
            Console.WriteLine($"Texture '{_handle}' disposed.");
            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }

    public enum TextureWrap
    {
        Repeat,
        Clamp
    }

    public enum TextureFilter
    {
        Linear,
        Point
    }
}