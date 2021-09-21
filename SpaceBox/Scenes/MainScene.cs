using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cubic.GUI;
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
    public class MainScene : Scene
    {
        private Camera _camera;
        private string _worldName;
        private SaveGame _save;

        public MainScene(SpaceboxGame game, string worldName = "", SaveGame save = null) : base(game)
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

        private List<Vector3> _cubeStuffs;

        private Texture2D _crosshair;

        private float _camSpeed;
        private bool _hide;

        public override void Initialize()
        {
            base.Initialize();
            
            GL.ClearColor(Color.Black);

            _camera = new Camera(new Vector3(0), Game.ClientSize.X / (float) Game.ClientSize.Y);
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
                _cubeStuffs = new List<Vector3>(_save.BlockPositions);
            }
            else
            {
                _cubeStuffs = new List<Vector3>();
                _cubeStuffs.Add(Vector3.Zero);
            }

            _cubeDist = 5;

            _input = SpaceboxGame.Config.Input;

            _crosshair = new Texture2D("Content/Textures/crosshair.png");

            _camSpeed = 2f;
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
                _camera.Position += _camSpeed * _camera.Front * Time.DeltaTime;
            if (Input.IsKeyDown(_input.MoveBackward))
                _camera.Position -= _camSpeed * _camera.Front * Time.DeltaTime;
            if (Input.IsKeyDown(_input.StrafeLeft))
                _camera.Position -= _camSpeed * _camera.Right * Time.DeltaTime;
            if (Input.IsKeyDown(_input.StrafeRight))
                _camera.Position += _camSpeed * _camera.Right * Time.DeltaTime;
            if (Input.IsKeyDown(_input.Jump))
                _camera.Position += _camSpeed * _camera.Up * Time.DeltaTime;
            if (Input.IsKeyDown(_input.CrouchOrJetpackDown))
                _camera.Position -= _camSpeed * _camera.Up * Time.DeltaTime;
            
            if (Input.IsKeyPressed(Keys.Escape))
                Game.Close();

            _camera.Yaw += Input.MouseDelta.X * mouseRot;
            _camera.Pitch -= Input.MouseDelta.Y * mouseRot;
            
            if (Input.IsMouseButtonPressed(MouseButton.Left))
                _cubeStuffs.Add(_cubeStuffs[0]);

            if (Input.IsKeyDown(Keys.LeftControl))
            {
                _camSpeed += Input.MouseState.ScrollDelta.Y;
                _camSpeed = MathHelper.Clamp(_camSpeed, 0, 2000);

                if (Input.IsKeyPressed(Keys.H) && _worldName != "")
                {
                    Data.SaveWorld(_worldName, _camera.Position, new Vector3(_camera.Yaw, _camera.Pitch, 0),
                        _cubeStuffs);
                    Console.WriteLine("Saved");
                }
            }
            else
            {
                _cubeDist += (int) Input.MouseState.ScrollDelta.Y;
                _cubeDist = MathHelper.Clamp(_cubeDist, 2, 12);
            }

            _cubeStuffs[0] = _camera.Position + _camera.Front * _cubeDist;

            bool looking = false;
            Vector3 lookingBlock = Vector3.Zero;
            int lookingBlockIndex = 0;
            
            /*for (float i = 0; i < _cubeDist; i += 0.05f)
            {
                Vector3 pos = _camera.Position + _camera.Front * i;
                
                for (int b = 0; b < _cubeStuffs.Count; b++)
                {
                    Vector3 vec = _cubeStuffs[b];
                    if (vec.X >= pos.X - 1f && vec.X <= pos.X + 1f &&
                        vec.Y >= pos.Y - 1f && vec.Y <= pos.Y + 1f &&
                        vec.Z >= pos.Z - 1f && vec.Z <= pos.Z + 1f && b != 0)
                    {
                        //Console.WriteLine(Vector3.Normalize(vec - _camera.Position));

                        //Console.WriteLine(Matrix4.Invert(Matrix4.CreateTranslation(pos)));
                        
                        //Vector3 invert = new Vector3(Matrix4.Invert(Matrix4.CreateTranslation(vec)).Row3);
                        
                        //Console.WriteLine(invert);

                        //Console.WriteLine(Vector3.Normalize(pos - vec));
                        
                        Vector3.Normalize(pos - vec).Deconstruct(out float x, out float y, out float z);

                        x = MathF.Round(x, 2);
                        y = MathF.Round(y, 2);
                        z = MathF.Round(z, 2);

                        float absX = MathF.Abs(x);
                        float absY = MathF.Abs(y);
                        float absZ = MathF.Abs(z);

                        Vector3 final = Vector3.Zero;
                        
                        if (absX > absY && absX > absZ)
                            if (x > 0)
                                final.X = 2;
                            else
                                final.X = -2;
                        else if (absY > absX && absY > absZ)
                            if (y > 0)
                                final.Y = 2;
                            else
                                final.Y = -2;
                        else if (absZ > absX && absZ > absY)
                            if (z > 0)
                                final.Z = 2;
                            else
                                final.Z = -2;

                        lookingBlock = vec + final;

                        looking = true;
                        lookingBlockIndex = b;

                        break;
                    }
                }

                if (looking)
                    break;
            }*/

            if (Physics.Raycast(pos =>
                {
                    //Console.WriteLine(pos);
                    //_cubeStuffs[0] = pos;
                    for (int b = 1; b < _cubeStuffs.Count; b++)
                    {
                        Vector3 vec = _cubeStuffs[b];
                        //Console.WriteLine(vec);
                        //Console.WriteLine(pos);
                        if (vec.X >= pos.X - 1f && vec.X <= pos.X + 1f &&
                            vec.Y >= pos.Y - 1f && vec.Y <= pos.Y + 1f &&
                            vec.Z >= pos.Z - 1f && vec.Z <= pos.Z + 1f)
                        {
                            //Console.WriteLine("Looking at");
                            lookingBlockIndex = b;
                            lookingBlock = vec;
                            looking = true;
                            return true;
                        }
                    }

                    //Console.WriteLine("Not looking at");
                    return false;
                }, 
                _camera.Position, _camera.Front, (uint) _cubeDist + 1, out RaycastHit hit))
            {
                //Console.WriteLine(hit.Position);
                //Console.WriteLine(hit.Normal);

                _cubeStuffs[0] = lookingBlock + hit.Normal * 20;
            }
            
            if (looking)
            {
                //_cubeStuffs[0] = lookingBlock;
                if (Input.IsMouseButtonPressed(MouseButton.Right))
                    _cubeStuffs.RemoveAt(lookingBlockIndex);
            }

            if (Input.IsKeyPressed(Keys.Tab))
                _hide = !_hide;
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

            int i = 0;
            foreach (Vector3 vec in _cubeStuffs)
            {
                if (i == 0 && _hide)
                {
                    i++;
                    continue;
                }

                _shader.SetUniform("uOpacity",  i == 0 ? 0.5f : 1f);

                GL.BindVertexArray(_vao);

                _shader.SetUniform("uModel", Matrix4.CreateTranslation(vec));

                //GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                i++;
            }

            if (!_hide)
            {
                Game.SpriteBatch.Begin();
                Game.SpriteBatch.Draw(_crosshair, new Vector2(Game.SpriteBatch.Width, Game.SpriteBatch.Height) / 2,
                    Color.White, 0, _crosshair.Size.ToVector2() / 2, new Vector2(0.05f) * Game.UiManager.UiScale);
                Game.SpriteBatch.End();
            }
        }
    }
}