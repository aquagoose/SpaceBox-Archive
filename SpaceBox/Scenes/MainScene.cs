using System.Drawing;
using Cubic.Render;
using Cubic.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Spacebox.Scenes
{
    public class MainScene : Scene
    {
        private Camera _camera;
        
        public MainScene(SpaceboxGame game) : base(game) { }

        private readonly float[] _vertices =
        {
            // Vertex    TexCoords 
            1f, 1f, 0.0f, 1.0f, 0.0f,
            1f, -1f, 0.0f, 1.0f, 1.0f,
            -1f, -1f, 0.0f, 0.0f, 1.0f,
            -1f, 1f, 0.0f, 0.0f, 0.0f
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        
        private int _vao;
        private int _vbo;
        private int _ebo;
        private Shader _shader;
        private Texture2D _texture;
        
        public override void Initialize()
        {
            base.Initialize();
            
            GL.ClearColor(Color.Black);

            _camera = new Camera(new Vector3(0), Game.ClientSize.X / (float) Game.ClientSize.Y);

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

            _shader = new Shader("Content/Shaders/Model.vert", "Content/Shaders/Model.frag");
            _shader.Use();

            int vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordsLocation = _shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordsLocation);
            GL.VertexAttribPointer(texCoordsLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));

            _texture = new Texture2D("Content/Textures/Blocks/grass.jpg");
            
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public override void Update()
        {
            base.Update();
            
            
        }

        public override void Draw()
        {
            base.Draw();
            
            _shader.Use();
            _shader.SetUniform("uModel", Matrix4.CreateTranslation(0, 0, -5));
            _shader.SetUniform("uView", _camera.ViewMatrix);
            _shader.SetUniform("uProjection", _camera.ProjectionMatrix);
            
            _texture.Bind();

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}