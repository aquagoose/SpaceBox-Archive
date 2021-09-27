using System;
using Cubic.Physics;
using Cubic.Render;
using Cubic.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceBox.Data;
using SpaceBox.Sandbox.Grids;
using SpaceBox.Sandbox.Utilities;
using SpaceBox.Sandbox.Worlds;

namespace Spacebox.Scenes
{
    public class MainScene : Scene
    {
        private readonly float[] _vertices =
        {
            -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -1.0f,  1.0f, -1.0f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  1.0f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             1.0f,  1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

        private PlayerCamera _camera;

        private int _vao;
        private int _vbo;

        private Shader _shader;
        private Texture2D _texture;
        private Skybox _skybox;

        private float _camSpeed;

        private InputConfig _input;

        public MainScene(SpaceboxGame game) : base(game) { }

        public override void Initialize()
        {
            base.Initialize();

            _camera = new PlayerCamera(Vector3.Zero, Vector3.Zero, Game.ClientSize.X / (float) Game.ClientSize.Y, 90,
                0.1f, 1000f);
            
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.StaticDraw);

            //_ebo = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
            //    BufferUsageHint.StaticDraw);

            _shader = new Shader("Content/Shaders/Model.vert", "Content/Shaders/Model.frag");
            _shader.Use();
            _shader.SetUniform("uMaterial.shininess", 16.0f);
            _shader.SetUniform("uDirLight.direction", new Vector3(-0.25f, 0.9f, -1f));
            _shader.SetUniform("uDirLight.ambient", new Vector3(0.3f));
            _shader.SetUniform("uDirLight.diffuse", new Vector3(0.7f));
            _shader.SetUniform("uDirLight.specular", new Vector3(1f));

            int vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            int texCoordsLocation = _shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordsLocation);
            GL.VertexAttribPointer(texCoordsLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float),
                6 * sizeof(float));
            
            int normalsLocation = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalsLocation);
            GL.VertexAttribPointer(normalsLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float),
                3 * sizeof(float));

            _texture = new Texture2D("Content/Textures/Blocks/stainless-steel.jpg");

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            Game.CursorGrabbed = true;
            
            GL.Enable(EnableCap.DepthTest);
            
            _skybox = new Skybox(new[]
            {
                "Content/Textures/Skybox/right.png",
                "Content/Textures/Skybox/left.png",
                "Content/Textures/Skybox/top.png",
                "Content/Textures/Skybox/bottom.png",
                "Content/Textures/Skybox/front.png",
                "Content/Textures/Skybox/back.png"
            });
            
            Physics.Initialize();

            _camSpeed = 2;

            _input = SpaceboxGame.Config.Input;

            World.Instance = new World();
        }

        public override void Update()
        {
            base.Update();
            
            const float rotSpeed = 100f;
            const float mouseRot = 0.3f;

            if (Input.IsKeyDown(Keys.Right))
                _camera.Rotation.X += rotSpeed * Time.DeltaTime;
            if (Input.IsKeyDown(Keys.Left))
                _camera.Rotation.X -= rotSpeed * Time.DeltaTime;
            if (Input.IsKeyDown(Keys.Up))
                _camera.Rotation.Y += rotSpeed * Time.DeltaTime;
            if (Input.IsKeyDown(Keys.Down))
                _camera.Rotation.Y -= rotSpeed * Time.DeltaTime;

            if (Input.IsKeyDown(_input.MoveForward))
                _camera.Position += _camSpeed * _camera.Forward * Time.DeltaTime;
            if (Input.IsKeyDown(_input.MoveBackward))
                _camera.Position -= _camSpeed * _camera.Forward * Time.DeltaTime;
            if (Input.IsKeyDown(_input.StrafeLeft))
                _camera.Position -= _camSpeed * _camera.Right * Time.DeltaTime;
            if (Input.IsKeyDown(_input.StrafeRight))
                _camera.Position += _camSpeed * _camera.Right * Time.DeltaTime;
            if (Input.IsKeyDown(_input.Jump))
                _camera.Position += _camSpeed * _camera.Up * Time.DeltaTime;
            if (Input.IsKeyDown(_input.CrouchOrJetpackDown))
                _camera.Position -= _camSpeed * _camera.Up * Time.DeltaTime;

            _camera.Rotation.X += Input.MouseDelta.X * mouseRot;
            _camera.Rotation.Y -= Input.MouseDelta.Y * mouseRot;
            
            _camera.Update();
            
            //Console.WriteLine(World.Instance.Grids.Count);
        }

        public override void Draw()
        {
            base.Draw();
            
            _skybox.Draw(_camera.CameraBase);
            
            _shader.Use();
            _texture.Bind();
            
            _shader.SetUniform("uView", _camera.CameraBase.ViewMatrix);
            _shader.SetUniform("uProjection", _camera.CameraBase.ProjectionMatrix);
            _shader.SetUniform("uViewPos", _camera.Position);
            
            _shader.SetUniform("uOpacity", 1f);
            
            GL.BindVertexArray(_vao);

            foreach (Grid grid in World.Instance.Grids)
            {
                Vector3 gridPos = grid.Position;
                Matrix4 gridRot = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(grid.Orientation));
                
                foreach (Block block in grid.Blocks)
                {
                    _shader.SetUniform("uModel", gridRot * Matrix4.CreateTranslation(gridPos + block.Coord * 2));
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }
            
            _shader.SetUniform("uOpacity", 0.75f);
            _shader.SetUniform("uModel",
                Matrix4.CreateFromQuaternion(_camera.PlaceCube.Rotation) *
                Matrix4.CreateTranslation(_camera.PlaceCube.Position));
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }
}