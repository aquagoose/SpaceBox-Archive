using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BepuPhysics;
using BepuPhysics.Collidables;
using Cubic.GUI;
using Cubic.Physics;
using Cubic.Render;
using Cubic.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceBox.Data;
using Image = Cubic.GUI.Image;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Spacebox.Scenes
{
    public class MainSceneOld : Scene
    {
        private Camera _camera;
        private string _worldName;
        private SaveGame _save;

        private Label _label;
        private float _startTime;
        private int _labelDuration;

        public MainSceneOld(SpaceboxGame game, string worldName = "", SaveGame save = null) : base(game)
        {
            _worldName = worldName;
            _save = save;
            if (save != null)
                _worldName = save.WorldName;
        }

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
        
        /*private readonly float[] _vertices =
        {
            // Vertex    TexCoords 
            -1, 1, -1, 0, 0, 0.0f,  0.0f, -1.0f,
            1, 1, -1, 1, 0, 0.0f,  0.0f, -1.0f,
            1, 1, 1, 1, 1, 0.0f,  0.0f, -1.0f,
            -1, 1, 1, 0, 1, 0.0f,  0.0f, -1.0f,
            
            -1, -1, 1, 0, 0, 0.0f,  0.0f,  1.0f,
            1, -1, 1, 1, 0, 0.0f,  0.0f,  1.0f,
            1, -1, -1, 1, 1, 0.0f,  0.0f,  1.0f,
            -1, -1, -1, 0, 1, 0.0f,  0.0f,  1.0f,
            
            -1, 1, -1, 0, 0, -1.0f,  0.0f,  0.0f,
            -1, 1, 1, 1, 0, -1.0f,  0.0f,  0.0f,
            -1, -1, 1, 1, 1, -1.0f,  0.0f,  0.0f,
            -1, -1, -1, 0, 1, -1.0f,  0.0f,  0.0f,
            
            1, 1, 1, 0, 0, 1.0f,  0.0f,  0.0f,
            1, 1, -1, 1, 0, 1.0f,  0.0f,  0.0f,
            1, -1, -1, 1, 1, 1.0f,  0.0f,  0.0f,
            1, -1, 1, 0, 1, 1.0f,  0.0f,  0.0f,
            
            1, 1, -1, 0, 0, 0.0f, -1.0f,  0.0f,
            -1, 1, -1, 1, 0, 0.0f, -1.0f,  0.0f,
            -1, -1, -1, 1, 1, 0.0f, -1.0f,  0.0f,
            1, -1, -1, 0, 1, 0.0f, -1.0f,  0.0f,
            
            -1, 1, 1, 0, 0, 0.0f,  1.0f,  0.0f,
            1, 1, 1, 1, 0, 0.0f,  1.0f,  0.0f,
            1, -1, 1, 1, 1, 0.0f,  1.0f,  0.0f,
            -1, -1, 1, 0, 1, 0.0f,  1.0f,  0.0f
        };

        private readonly uint[] _indices =
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        };*/

        private InputConfig _input;
        
        private int _vao;
        private int _vbo;
        private int _ebo;
        private Shader _shader;
        private Texture2D _texture;
        private Skybox _skybox;

        private int _cubeDist;

        private List<Vector3> _cubePositions;
        private List<Quaternion> _cubeRotations;
        private Vector3 _placeCube;
        private Quaternion _placeCubeRotation;

        private Texture2D _crosshair;

        private float _camSpeed;
        private bool _hide;

        public override void Initialize()
        {
            base.Initialize();
            
            GL.ClearColor(Color.Black);

            _camera = new Camera(Vector3.Zero, Vector3.Zero, Game.ClientSize.X / (float) Game.ClientSize.Y, 90, 0.1f,
                1000f);
            if (_save != null)
            {
                _camera.Position = _save.PlayerPosition;
                _camera.Yaw = _save.PlayerRotation.X;
                _camera.Pitch = _save.PlayerRotation.Y;
            }

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
            //_texture = new Texture2D(@"D:\Users\ollie\Downloads\testthing.jpg");
            
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            Game.CursorGrabbed = true;
            
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.CullFace);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.CullFace(CullFaceMode.Back);

            _skybox = new Skybox(new[]
            {
                "Content/Textures/Skybox/right.png",
                "Content/Textures/Skybox/left.png",
                "Content/Textures/Skybox/top.png",
                "Content/Textures/Skybox/bottom.png",
                "Content/Textures/Skybox/front.png",
                "Content/Textures/Skybox/back.png"
            });

            if (_save != null)
            {
                _cubePositions = new List<Vector3>(_save.BlockPositions);
            }
            else
            {
                _cubePositions = new List<Vector3>();
                _cubeRotations = new List<Quaternion>();
            }

            _cubeDist = 5;

            _input = SpaceboxGame.Config.Input;

            _crosshair = new Texture2D("Content/Textures/crosshair.png");

            _camSpeed = 2f;

            _label = new Label(Game.UiManager, new Position(DockType.Center, Vector2.Zero))
            {
                Visible = false
            };
            
            Game.UiManager.Add("label", _label);
            
            Physics.Initialize();

            ShowLabel("Hi! Welcome to SpaceBox.", 5);

            Quaternion quat = Quaternion.FromEulerAngles(100, 0, 0);
            _cubeRotations.Add(quat);
            
            _placeCubeRotation = quat;
            
            Physics.Simulation.Statics.Add(new StaticDescription(System.Numerics.Vector3.Zero, new System.Numerics.Quaternion(quat.X, quat.Y, quat.Z, quat.W),
                new CollidableDescription(Physics.Simulation.Shapes.Add(new Box(2, 2, 2)), 0.1f)));
        }

        public override void Update()
        {
            base.Update();
            
            const float rotSpeed = 100f;
            const float mouseRot = 0.3f;

            if (Input.IsKeyDown(Keys.Right))
                _camera.Yaw += rotSpeed * Time.DeltaTime;
            if (Input.IsKeyDown(Keys.Left))
                _camera.Yaw -= rotSpeed * Time.DeltaTime;
            if (Input.IsKeyDown(Keys.Up))
                _camera.Pitch += rotSpeed * Time.DeltaTime;
            if (Input.IsKeyDown(Keys.Down))
                _camera.Pitch -= rotSpeed * Time.DeltaTime;

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

            _camera.Yaw += Input.MouseDelta.X * mouseRot;
            _camera.Pitch -= Input.MouseDelta.Y * mouseRot;
            
            if (Input.IsMouseButtonPressed(MouseButton.Left))
                _cubePositions.Add(_placeCube);

            if (Input.IsKeyDown(Keys.LeftControl))
            {
                _camSpeed += Input.MouseState.ScrollDelta.Y;
                _camSpeed = MathHelper.Clamp(_camSpeed, 0, 2000);
            }
            else
            {
                _cubeDist += (int) Input.MouseState.ScrollDelta.Y;
                _cubeDist = MathHelper.Clamp(_cubeDist, 2, 30);
            }
            
            if (Input.IsKeyPressed(_input.SaveGame) && _worldName != "")
            {
                Data.SaveWorld(_worldName, _camera.Position, new Vector3(_camera.Yaw, _camera.Pitch, 0),
                    _cubePositions);
                Console.WriteLine("Saved");
                ShowLabel("Saved game.", 5);
            }

            _placeCube = _camera.Position + _camera.Forward * _cubeDist;

            bool looking = false;
            Vector3 lookingBlock = Vector3.Zero;
            int lookingBlockIndex = 0;

            if (Physics.Raycast(_camera.Position, _camera.Forward, _cubeDist, out RaycastHit hit))
            {
                _placeCube = hit.Position + hit.Normal * 2;
                //_placeCube = hit.Position + normalizedVec;
            }

            if (Input.IsKeyPressed(Keys.Tab))
                _hide = !_hide;
            
            if (Input.IsKeyPressed(_input.TakeScreenshot))
                ShowLabel("Screenshot saved.", 5);
            
            if (Input.IsKeyPressed(Keys.Escape))
                Game.SetScene(new MenuScene(Game));

            if (_label.Visible)
            {
                if (Time.ElapsedSeconds - _startTime >= _labelDuration)
                {
                    _label.Visible = false;
                    _label.Text = "";
                }
            }

            _cubePositions.Add(Vector3.Zero);

            Physics.Simulate(1 / 60f);
        }

        public override void Draw()
        {
            base.Draw();
            
            _skybox.Draw(_camera);
            
            _shader.Use();
            _texture.Bind();
            
            _shader.SetUniform("uView", _camera.ViewMatrix);
            _shader.SetUniform("uProjection", _camera.ProjectionMatrix);
            _shader.SetUniform("uViewPos", _camera.Position);
            
            _shader.SetUniform("uOpacity", 1f);
            
            GL.BindVertexArray(_vao);
            
            for (int i = 0; i < _cubeRotations.Count; i++)
            {

                _shader.SetUniform("uModel",
                    Matrix4.CreateFromQuaternion(_cubeRotations[i]) * Matrix4.CreateTranslation(_cubePositions[i]));

                //GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            if (!_hide)
            {

                _shader.SetUniform("uOpacity", 0.75f);
                _shader.SetUniform("uModel",
                    Matrix4.CreateFromQuaternion(_placeCubeRotation) * Matrix4.CreateTranslation(_placeCube));

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            if (!_hide)
            {
                Game.SpriteBatch.Begin();
                Game.SpriteBatch.Draw(_crosshair, new Vector2(Game.SpriteBatch.Width, Game.SpriteBatch.Height) / 2,
                    Color.White, 0, _crosshair.Size.ToVector2() / 2, new Vector2(0.05f) * Game.UiManager.UiScale);
                Game.SpriteBatch.End();
                Game.UiManager.Draw();
            }
        }

        public void ShowLabel(string text, int duration)
        {
            const float labelOffsetY = 300;
            _label.Text = text;
            _label.Position = new Position(DockType.Center,
                -_label.MeasureString(text) / 2 + new Vector2(0, labelOffsetY));
            _label.Visible = true;
            _startTime = Time.ElapsedSeconds;
            _labelDuration = duration;
        }
        
    }
}