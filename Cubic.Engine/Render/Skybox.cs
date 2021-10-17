using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Cubic.Engine.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Cubic.Engine.Render
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
        
        public Skybox(Shader skyboxShader, Bitmap[] textures)
        {
            _shader = skyboxShader;
            
            _texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, _texture);

            for (int i = 0; i < textures.Length; i++)
            {
                Bitmap bp = new Bitmap(textures[i]);
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    bp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                BitmapData data = bp.LockBits(new Rectangle(0, 0, bp.Width, bp.Height), ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);
                
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, textures[i].Width,
                        textures[i].Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                
                bp.UnlockBits(data);
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

            //_shader = new Shader("Content/Shaders/Skybox.vert", "Content/Shaders/Skybox.frag");
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
            _shader.SetUniform<Matrix4>("uProjection", camera.ProjectionMatrix);

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