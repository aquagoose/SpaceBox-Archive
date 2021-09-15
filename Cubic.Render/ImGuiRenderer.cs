using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cubic.Utilities;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Cubic.Render
{
    // Adapted from
    // https://github.com/NogginBops/ImGui.NET_OpenTK_Sample/blob/opengl3.3/Dear%20ImGui%20Sample/ImGuiController.cs
    public class ImGuiRenderer : IDisposable
    {
        private int _windowWidth;
        private int _windowHeight;

        private bool _frameBegun;

        private int _vao;
        private int _vbo;
        private int _ebo;

        private int _vboSize;
        private int _eboSize;

        private Shader _shader;

        private Texture2D _fontTexture;

        private Vector2 _scaleFactor;

        private readonly List<char> _pressedChars;

        private Keys[] _keysList;

        public ImGuiRenderer(NativeWindow window)
        {
            _scaleFactor = Vector2.One;
            
            _windowWidth = window.ClientSize.X;
            _windowHeight = window.ClientSize.Y;
            
            window.Resize += WindowOnResize;
            window.TextInput += PressChar;

            _pressedChars = new List<char>();
            _keysList = (Keys[])Enum.GetValues(typeof(Keys));

            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.AddFontDefault();
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            CreateDeviceResources();
            SetKeyMappings();

            SetPerFrameImGuiData(1f / 60f);
            
            ImGui.NewFrame();
            _frameBegun = true;
        }

        private void WindowOnResize(ResizeEventArgs obj)
        {
            _windowWidth = obj.Width;
            _windowHeight = obj.Height;
        }

        public void DestroyDeviceObjects()
        {
            Dispose();
        }

        public void CreateDeviceResources()
        {
            _vao = GL.GenVertexArray();
            
            _vboSize = 10000;
            _eboSize = 2000;

            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vboSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ArrayBuffer, _eboSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            RecreateFontDeviceTexture();

            const string vertexSource = @"
#version 330 core

in vec2 aPosition;
in vec2 aTexCoords;
in vec4 aColor;

out vec4 frag_color;
out vec2 frag_texCoords;

uniform mat4 uProjection;

void main()
{
    gl_Position = uProjection * vec4(aPosition, 0, 1);
    frag_color = aColor;
    frag_texCoords = aTexCoords;
}";

            const string fragmentSource = @"
#version 330 core

in vec4 frag_color;
in vec2 frag_texCoords;

out vec4 out_color;

uniform sampler2D uTexture;

void main()
{
    out_color = frag_color * texture(uTexture, frag_texCoords);
}";

            _shader = new Shader(vertexSource, fragmentSource, ShaderLoadType.String, false);
            _shader.Use();
            
            GL.BindVertexArray(_vao);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

            int stride = Unsafe.SizeOf<ImDrawVert>();

            int vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, stride, 0);

            int texCoordLocation = _shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, stride, 8);

            int colorLocation = _shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);
            
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

            _fontTexture = new Texture2D(width, height, pixels, mipmap: false, autoDispose: false);
            
            io.Fonts.SetTexID((IntPtr) _fontTexture.Handle);
            
            io.Fonts.ClearTexData();
        }

        public void Render()
        {
            if (_frameBegun)
            {
                _frameBegun = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());
            }
        }

        public void Update(float deltaSeconds)
        {
            if (_frameBegun)
                ImGui.Render();

            SetPerFrameImGuiData(deltaSeconds);
            UpdateImGuiInput();

            _frameBegun = true;
            
            ImGui.NewFrame();
        }

        private void SetPerFrameImGuiData(float deltaSeconds)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new Vector2(_windowWidth / _scaleFactor.X, _windowHeight / _scaleFactor.Y);
            io.DisplayFramebufferScale = _scaleFactor;
            io.DeltaTime = deltaSeconds;
        }

        private void UpdateImGuiInput()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            io.MouseDown[0] = Input.IsMouseButtonDown(MouseButton.Left);
            io.MouseDown[1] = Input.IsMouseButtonDown(MouseButton.Right);
            io.MouseDown[2] = Input.IsMouseButtonDown(MouseButton.Middle);

            io.MousePos = Input.MousePosition.ToSystemNumericsVector2();

            io.MouseWheel = Input.MouseScroll.Y - Input.MouseState.PreviousScroll.Y;
            io.MouseWheelH = Input.MouseScroll.X - Input.MouseState.PreviousScroll.X;

            foreach (Keys key in _keysList)
            {
                if ((int) key > 0)
                    io.KeysDown[(int) key] = Input.IsKeyDown(key);
            }
            
            foreach (char c in _pressedChars)
                io.AddInputCharacter(c);
            _pressedChars.Clear();
            
            io.KeyCtrl = Input.IsKeyDown(Keys.LeftControl) || Input.IsKeyDown(Keys.RightControl);
            io.KeyAlt = Input.IsKeyDown(Keys.LeftAlt) || Input.IsKeyDown(Keys.RightAlt);
            io.KeyShift = Input.IsKeyDown(Keys.LeftShift) || Input.IsKeyDown(Keys.RightShift);
            io.KeySuper = Input.IsKeyDown(Keys.LeftSuper) || Input.IsKeyDown(Keys.RightSuper);
        }

        private void PressChar(TextInputEventArgs obj)
        {
            _pressedChars.Add((char) obj.Unicode);
        }

        private static void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
        }

        private void RenderImDrawData(ImDrawDataPtr drawData)
        {
            uint vertexOffsetInVertices = 0;
            uint indexOffsetInElements = 0;

            if (drawData.CmdListsCount == 0)
                return;
            
            uint totalVbSize = (uint) (drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
            if (totalVbSize > _vboSize)
            {
                _vboSize = (int) Math.Max(_vboSize * 1.5f, totalVbSize);
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _vboSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            uint totalIbSize = (uint) (drawData.TotalIdxCount * sizeof(ushort));
            if (totalIbSize > _eboSize)
            {
                _eboSize = (int) Math.Max(_eboSize * 1.5f, totalIbSize);
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, _ebo);
                GL.BufferData(BufferTarget.ArrayBuffer, _eboSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            for (int i = 0; i < drawData.CmdListsCount; i++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[i];
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferSubData(BufferTarget.ArrayBuffer,
                    (IntPtr) (vertexOffsetInVertices * Unsafe.SizeOf<ImDrawVert>()),
                    cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmdList.VtxBuffer.Data);
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, _ebo);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr) (indexOffsetInElements * sizeof(ushort)),
                    cmdList.IdxBuffer.Size * sizeof(ushort), cmdList.IdxBuffer.Data);

                vertexOffsetInVertices += (uint) cmdList.VtxBuffer.Size;
                indexOffsetInElements += (uint) cmdList.IdxBuffer.Size;
            }
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            ImGuiIOPtr io = ImGui.GetIO();

            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(0.0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1, 1);
            _shader.Use();
            _shader.SetUniform("uProjection", mvp, false);
            _shader.SetUniform("uTexture", 0);
            
            GL.BindVertexArray(_vao);
            
            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            bool wasBlendEnabled = GL.IsEnabled(EnableCap.Blend);
            bool wasScissorEnabled = GL.IsEnabled(EnableCap.ScissorTest);
            bool wasCullingEnabled = GL.IsEnabled(EnableCap.CullFace);
            bool wasDepthTestEnabled = GL.IsEnabled(EnableCap.DepthTest);
            
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];
                for (int cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
                {
                    ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdI];
                    if (pcmd.UserCallback != IntPtr.Zero)
                        throw new NotImplementedException();
                    
                    _fontTexture.Bind();

                    Vector4 clipRect = pcmd.ClipRect;
                    GL.Scissor((int) clipRect.X, _windowHeight - (int) clipRect.W, (int) (clipRect.Z - clipRect.X),
                        (int) (clipRect.W - clipRect.Y));

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int) pcmd.ElemCount,
                        DrawElementsType.UnsignedShort, (IntPtr) (idxOffset * sizeof(ushort)), vtxOffset);

                    idxOffset += (int) pcmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }
            
            if (!wasBlendEnabled)
                GL.Disable(EnableCap.Blend);
            if (!wasScissorEnabled)
                GL.Disable(EnableCap.ScissorTest);
            if (wasCullingEnabled)
                GL.Enable(EnableCap.CullFace);
            if (wasDepthTestEnabled)
                GL.Enable(EnableCap.DepthTest);
        }
        
        public void Dispose()
        {
            _fontTexture.Dispose();
            _shader.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}