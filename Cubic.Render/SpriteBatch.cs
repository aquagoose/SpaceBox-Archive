using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Cubic.Render
{
    public class SpriteBatch : IDisposable
    {
        public event OnResized Resized;
        
        // All a Sprite really is, is a textured quad!
        // It's still rendered in 3D space, the GPU only supports rendering in 3D.
        private readonly float[] _vertices =
        {
         // Vertex    TexCoords 
            1f, 1f,   1.0f, 0.0f,
            1f, -1f,  1.0f, 1.0f,
            -1f, -1f, 0.0f, 1.0f,
            -1f, 1f,  0.0f, 0.0f
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        // Vertex array object
        private int _vao;
        private int _vbo;
        private int _ebo;

        private Shader _spriteShader;

        private Shader _activeShader;
        
        public int Width { get; private set; }
        public int Height { get; private set; }

        private bool _begun;
        
        public SpriteBatch(NativeWindow window)
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _spriteShader = new Shader("Content/Shaders/2D/SpriteBatch.vert", "Content/Shaders/2D/SpriteBatch.frag", autoDispose: false);
            _spriteShader.Use();
            _spriteShader.SetUniform("uProjection",
                Matrix4.CreateOrthographicOffCenter(0, window.ClientSize.X, window.ClientSize.Y, 0, -1, 1));
            _spriteShader.SetUniform("uTransform", Matrix4.Identity);

            int vertexLocation = _spriteShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            int texCoordLocation = _spriteShader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float),
                2 * sizeof(float));

            _activeShader = _spriteShader;
            
            //GL.BindVertexArray(0);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            window.Resize += WindowOnResize;
            Width = window.ClientSize.X;
            Height = window.ClientSize.Y;
        }

        private void WindowOnResize(ResizeEventArgs e)
        {
            _activeShader.Use();
            _activeShader.SetUniform("uProjection",
                Matrix4.CreateOrthographicOffCenter(0, e.Width, e.Height, 0, -1, 1));
            Width = e.Width;
            Height = e.Height;
            Resized?.Invoke();
        }

        public void Begin(Matrix4 transform = default, Shader shader = null)
        {
            if (_begun)
                throw new Exception("SpriteBatch End() must be called before Begin() can be called again.");
            _begun = true;
            _activeShader = shader ?? _spriteShader;
            _activeShader.Use();
            _activeShader.SetUniform("uTransform", transform == default ? Matrix4.Identity : transform);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void End()
        {
            if (!_begun)
                throw new Exception("SpriteBatch Begin() must be called before End() can be called.");
            _begun = false;
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Draw a sprite to the screen.
        /// </summary>
        /// <param name="texture">The sprite's texture.</param>
        /// <param name="position">The sprite's position.</param>
        /// <param name="color">The sprite's colour tint.</param>
        /// <param name="rotation">The sprite's rotation, in radians.</param>
        /// <param name="origin">The origin point of the sprite.</param>
        /// <param name="scale">The scale of the sprite.</param>
        public void Draw(Texture2D texture, Vector2 position, Color color, float rotation, Vector2 origin,
            Vector2 scale)
        {
            if (!_begun)
                throw new Exception("SpriteBatch Begin() must be called before Draw() can be called.");
            
            _activeShader.Use();
            texture.Bind();
            // These matrices attempt to replicate the MonoGame/XNA SpriteBatch.
            // For some reason, the screen scale is always twice as big as it should be, however dividing it by 2 seems
            // to work, for some reason.
            Vector2 screenScale = new Vector2(texture.Width * scale.X, texture.Height * scale.Y) / 2f;
            Matrix4 model = Matrix4.CreateScale(screenScale.X, screenScale.Y, 1) *
                            Matrix4.CreateTranslation(screenScale.X, screenScale.Y, 0) *
                            Matrix4.CreateTranslation(-origin.X * scale.X, -origin.Y * scale.Y, 0) *
                            Matrix4.CreateRotationZ(rotation) *
                            Matrix4.CreateTranslation(new Vector3(position));
            _activeShader.SetUniform("uModel", model);
            _activeShader.SetUniform("uColor", color);
            
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            _spriteShader.Dispose();
            Console.WriteLine("SpriteBatch disposed.");
        }

        public delegate void OnResized();
    }
}