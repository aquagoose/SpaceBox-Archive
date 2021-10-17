using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Cubic.Engine.Render
{
    public class ShadowMap
    {
        private int _depthMapFbo;
        private int _depthMapTexture;
        private int[] _viewport;

        private Size _texSize;

        public Shader Shader { get; }

        public ShadowMap(Size mapSize)
        {
            _depthMapFbo = GL.GenFramebuffer();

            _depthMapTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _depthMapTexture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, mapSize.Width, mapSize.Height,
                0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _depthMapFbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D, _depthMapTexture, 0);
            GL.DrawBuffer(0);
            GL.ReadBuffer(0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            Shader = new Shader("Content/Shaders/DepthMap.vert", "Content/Shaders/DepthMap.frag");

            _viewport = new int[4];
            _texSize = mapSize;
        }

        public void Bind()
        {
            GL.GetInteger(GetPName.Viewport, _viewport);
            
            GL.Viewport(0, 0, _texSize.Width, _texSize.Height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _depthMapFbo);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-10.0f, 10.0f, -10.0f, 10.0f, 1.0f, 200f);
            Matrix4 lightView = Matrix4.LookAt(new Vector3(0.25f * 100, -0.9f * 100, 1f * 100), Vector3.Zero, Vector3.UnitY);
            //Matrix4 lightView = Matrix4.LookAt(new Vector3(0, 0, 5), new Vector3(0), new Vector3(0.0f, 1.0f, 0.0f));
            Shader.Use();
            Shader.SetUniform("uLightSpace", lightView * lightProjection);
        }

        public void Free()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(_viewport[0], _viewport[1], _viewport[2], _viewport[3]);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void BindTexture(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _depthMapTexture);
        }
    }
}