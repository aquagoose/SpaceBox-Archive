using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cubic.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Cubic.Render
{
    public class Skybox : IDisposable
    {
        private int _texture;
        
        private readonly float[] _vertices =
        {
            // Vertex    TexCoords 
            -1, 1, -1,
            1, 1, -1,
            1, 1, 1,
            -1, 1, 1,
            
            -1, -1, 1,
            1, -1, 1,
            1, -1, -1,
            -1, -1, -1,
            
            -1, 1, -1,
            -1, 1, 1,
            -1, -1, 1,
            -1, -1, -1,
            
            1, 1, 1,
            1, 1, -1,
            1, -1, -1,
            1, -1, 1,
            
            1, 1, -1,
            -1, 1, -1,
            -1, -1, -1,
            1, -1, -1,
            
            -1, 1, 1,
            1, 1, 1,
            1, -1, 1,
            -1, -1, 1,
        };

        private readonly uint[] _indices =
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        };

        private int _vao;
        private int _vbo;
        private int _ebo;
        private Shader _shader;
        
        public Skybox(string[] textures)
        {
            _texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, _texture);

            for (int i = 0; i < textures.Length; i++)
            {
                using (Bitmap bitmap = Path.GetExtension(textures[i]) == ".ctf"
                    ? Texture2D.LoadCTF(textures[i])[0]
                    : new Bitmap(textures[i]))
                {
                    BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, bitmap.Width,
                        bitmap.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                }
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR,
                (int) TextureWrapMode.ClampToEdge);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
                BufferUsageHint.StaticDraw);

            _shader = new Shader("Content/Shaders/Skybox.vert", "Content/Shaders/Skybox.frag");
            _shader.Use();

            int vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public void Draw(Camera camera)
        {
            GL.Disable(EnableCap.CullFace);
            GL.DepthMask(false);
            _shader.Use();
            _shader.SetUniform("uView", new Matrix4(new Matrix3(camera.ViewMatrix)));
            _shader.SetUniform("uProjection", camera.ProjectionMatrix);

            GL.BindVertexArray(_vao);
            GL.BindTexture(TextureTarget.TextureCubeMap, _texture);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            
            GL.DepthMask(true);
            //GL.Enable(EnableCap.CullFace);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
        }
    }
}