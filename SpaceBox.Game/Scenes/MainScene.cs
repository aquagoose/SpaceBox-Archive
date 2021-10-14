using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cubic.Data;
using Cubic.Physics;
using Cubic.Render;
using Cubic.Utilities;
using Cubic.Windowing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceBox.Data;
using SpaceBox.Data.Serialization;
using SpaceBox.Sandbox.Grids;
using SpaceBox.Sandbox.Utilities;
using SpaceBox.Sandbox.Worlds;

namespace Spacebox.Game.Scenes
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
        
        private readonly float[] _shadowVertices =
        {
            // Vertex
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
            -1, -1, 1
        };
        private readonly uint[] _shadowIndices =
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        };

        private PlayerCamera _camera;

        private int _vao;
        private int _vbo;

        private int _shadowVbo;
        private int _shadowVao;
        private int _shadowEbo;

        private Shader _shader;
        private Texture2D _texture;
        private Skybox _skybox;

        private Texture2D _crosshair;

        private float _camSpeed;

        private InputConfig _input;

        private string _worldName;
        private SaveGame _save;

        private ShadowMap _shadowMap;

        private bool _hideUi;

        public MainScene(string worldName = "", SaveGame save = null)
        {
            _worldName = worldName;
            if (save != null)
            {
                _save = save;
                _worldName = save.WorldName;
            }
        }

        public override void Initialize(SpaceboxGame game)
        {
            base.Initialize(game);

            _camera = new PlayerCamera(Vector3.Zero, Quaternion.Identity, SpriteBatch.Width / (float) SpriteBatch.Height, 90,
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
            
            _shadowMap = new ShadowMap(new Size(2048, 2048));

            _shader = new Shader("Content/Shaders/ModelBK.vert", "Content/Shaders/ModelBK.frag");
            _shader.Use();
            _shader.SetUniform("uMaterial.shininess", 16.0f);
            _shader.SetUniform("uDirLight.direction", new Vector3(-0.25f, 0.9f, -1f));
            _shader.SetUniform("uDirLight.ambient", new Vector3(0.3f));
            _shader.SetUniform("uDirLight.diffuse", new Vector3(0.7f));
            _shader.SetUniform("uDirLight.specular", new Vector3(1f));
            //_shader.SetUniform("uShadowMap", 1);

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

            _texture = new Texture2D(Content.LoadedTextures["stainless-steel"][0]);
            _crosshair = new Texture2D(Content.LoadedTextures["crosshair"][0]);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            _shadowVao = GL.GenVertexArray();
            GL.BindVertexArray(_shadowVao);

            _shadowVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _shadowVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _shadowVertices.Length * sizeof(float), _shadowVertices,
                BufferUsageHint.StaticDraw);

            _shadowEbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _shadowEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _shadowIndices.Length * sizeof(uint), _shadowIndices,
                BufferUsageHint.StaticDraw);

            int v2Location = _shadowMap.Shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(v2Location);
            GL.VertexAttribPointer(v2Location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            Input.CursorState = CursorState.Captured;
            
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            _skybox = new Skybox(new Shader("Content/Shaders/Skybox.vert", "Content/Shaders/Skybox.frag"), new[]
            {
                Content.LoadedTextures["right"][0],
                Content.LoadedTextures["left"][0],
                Content.LoadedTextures["top"][0],
                Content.LoadedTextures["bottom"][0],
                Content.LoadedTextures["front"][0],
                Content.LoadedTextures["back"][0]
            });
            
            Physics.Initialize();

            _camSpeed = 2;

            _input = SpaceboxGame.Config.Input;

            World.CurrentWorld = new World();

            if (_save != null)
            {
                List<Block> blocks = new List<Block>();
                
                _camera.Position = _save.PlayerPosition.ToNormal();
                _camera.Rotation = _save.PlayerRotation.ToNormal();
                
                foreach (SerializableGrid grid in _save.Grids)
                {
                    foreach (SerializableBlock block in grid.Blocks)
                    {
                        blocks.Add(new Block(block.Coord.ToNormal()) { Name = block.Name });
                    }

                    Grid newGrid = new Grid(grid.Position.ToNormal(), grid.Orientation.ToNormal(), grid.GridType,
                        grid.GridSize);
                    // I have to do this terribleness in order for the blocks not get deleted when I clear the list....
                    newGrid.Blocks = blocks.ToList();
                    newGrid.GeneratePhysics();
                    
                    World.CurrentWorld.Grids.Add(newGrid);
                    blocks.Clear();
                }
            }
            
            //Input.RegisterAction("jPressed", new InputAction(new []{ Keys.J, Keys.D7 }));
        }

        public override void Update(SpaceboxGame game)
        {
            base.Update(game);

            const float rotSpeed = 1f;
            const float mouseRot = 0.01f;

            //if (Input.IsKeyDown(Keys.Right))
            //    _camera.Rotation.X += rotSpeed * Time.DeltaTime;
            //if (Input.IsKeyDown(Keys.Left))
            //    _camera.Rotation.X -= rotSpeed * Time.DeltaTime;
            //if (Input.IsKeyDown(Keys.Up))
            //    _camera.Rotation.Y += rotSpeed * Time.DeltaTime;
            //if (Input.IsKeyDown(Keys.Down))
            //    _camera.Rotation.Y -= rotSpeed * Time.DeltaTime;
            
            if (Input.IsKeyDown(Keys.Q))
                _camera.Rotation *= Quaternion.FromAxisAngle(-Vector3.UnitZ, rotSpeed * Time.DeltaTime);
            if (Input.IsKeyDown(Keys.E))
                _camera.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, rotSpeed * Time.DeltaTime);

            if (Input.IsKeyDown(_input.MoveForward))
                _camera.Position += _camSpeed * _camera.Forward * Time.DeltaTime;
            if (Input.IsKeyDown(_input.MoveBackward))
                _camera.Position -= _camSpeed * _camera.Forward * Time.DeltaTime;
            if (Input.IsKeyDown(_input.StrafeLeft))
                _camera.Position += _camSpeed * _camera.Right * Time.DeltaTime;
            if (Input.IsKeyDown(_input.StrafeRight))
                _camera.Position -= _camSpeed * _camera.Right * Time.DeltaTime;
            if (Input.IsKeyDown(_input.Jump))
                _camera.Position += _camSpeed * _camera.Up * Time.DeltaTime;
            if (Input.IsKeyDown(_input.CrouchOrJetpackDown))
                _camera.Position -= _camSpeed * _camera.Up * Time.DeltaTime;
            
            _camera.Rotation *= Quaternion.Normalize(Quaternion.FromAxisAngle(Vector3.UnitX, Input.MouseDelta.Y * mouseRot));
            _camera.Rotation *= Quaternion.Normalize(Quaternion.FromAxisAngle(Vector3.UnitY, -Input.MouseDelta.X * mouseRot));

            if (Input.IsKeyDown(Keys.LeftControl))
                _camSpeed += (int) Input.ScrollDelta.Y;
            else
                _camera.PlaceCubeDistance += (int) Input.ScrollDelta.Y;

            if (Input.IsKeyDown(Keys.PageDown))
                _camera.PlaceCube.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitY, 1 * Time.DeltaTime);
            if (Input.IsKeyDown(Keys.Delete))
                _camera.PlaceCube.Rotation *= Quaternion.FromAxisAngle(-Vector3.UnitY, 1 * Time.DeltaTime);
            if (Input.IsKeyDown(Keys.Home))
                _camera.PlaceCube.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, 1 * Time.DeltaTime);
            if (Input.IsKeyDown(Keys.End))
                _camera.PlaceCube.Rotation *= Quaternion.FromAxisAngle(-Vector3.UnitZ, 1 * Time.DeltaTime);

            if (Input.IsKeyPressed(Keys.Escape))
            {
                Data.SaveWorld(_worldName, _camera.Position, _camera.Rotation, World.CurrentWorld.Grids);
                SceneManager.SetScene(new MenuScene());
            }
            
            //if (Input.IsActionHeld("jPressed"))
            //    Console.WriteLine("J is pressed!");

            if (Input.IsKeyPressed(Keys.Tab))
                _hideUi = !_hideUi;

            _camera.Update();
            
            if (Input.IsKeyPressed(_input.SaveGame) && _worldName != "")
                Data.SaveWorld(_worldName, _camera.Position, _camera.Rotation, World.CurrentWorld.Grids);

            World.CurrentWorld.Update();

            Physics.Simulate(1 / 60f);
        }

        public override void Draw(SpaceboxGame game)
        {
            base.Draw(game);

            //GL.Enable(EnableCap.CullFace);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.CullFace(CullFaceMode.Front);
            
            /*GL.BindVertexArray(_shadowVao);
            
            _shadowMap.Bind();

            foreach (Grid grid in World.CurrentWorld.Grids)
            {
                Vector3 gridPos = grid.Position;
                Matrix4 gridRot = Matrix4.CreateFromQuaternion(grid.Orientation);
                
                foreach (Block block in grid.Blocks)
                {
                    _shadowMap.Shader.SetUniform("uModel", Matrix4.CreateTranslation(block.Coord * 2) * gridRot * Matrix4.CreateTranslation(gridPos));
                    //_shader.SetUniform("uModel", gridRot * Matrix4.CreateTranslation(gridPos) * Matrix4.CreateTranslation(block.Coord * 2));
                    GL.DrawElements(PrimitiveType.Triangles, _shadowIndices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
            
            //if (_hideUi)
            //    return;

            //_shader.SetUniform("uOpacity", 0.75f);
            _shadowMap.Shader.SetUniform("uModel",
                Matrix4.CreateFromQuaternion(_camera.PlaceCube.Rotation) *
                Matrix4.CreateTranslation(_camera.PlaceCube.Position));
            GL.DrawElements(PrimitiveType.Triangles, _shadowIndices.Length, DrawElementsType.UnsignedInt, 0);
            
            _shadowMap.Free();
            
            //GL.Disable(EnableCap.CullFace);*/

            _skybox.Draw(_camera);
            
            _shader.Use();
            _texture.Bind();
            //_shadowMap.BindTexture(TextureUnit.Texture1);
            
            _shader.SetUniform("uView", _camera.ViewMatrix);
            _shader.SetUniform("uProjection", _camera.ProjectionMatrix);
            _shader.SetUniform("uViewPos", _camera.Position);

            //Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-10.0f, 10.0f, -10.0f, 10.0f, 1.0f, 200f);
            //Matrix4 lightView = Matrix4.LookAt(new Vector3(0.25f * 100, -0.9f * 100, 1f * 100), Vector3.Zero, Vector3.UnitY);
            //_shader.SetUniform("uLightSpace", lightView * lightProjection);

            //_shader.SetUniform("uProjection", lightProjection);
            //_shader.SetUniform("uView", lightView);
            
            _shader.SetUniform("uOpacity", 1f);
            
            GL.BindVertexArray(_vao);
            
            foreach (Grid grid in World.CurrentWorld.Grids)
            {
                Vector3 gridPos = grid.Position;
                Matrix4 gridRot = Matrix4.CreateFromQuaternion(grid.Orientation);
                
                foreach (Block block in grid.Blocks)
                {
                    _shader.SetUniform("uModel", Matrix4.CreateTranslation(block.Coord * 2) * gridRot * Matrix4.CreateTranslation(gridPos));
                    //_shader.SetUniform("uModel", gridRot * Matrix4.CreateTranslation(gridPos) * Matrix4.CreateTranslation(block.Coord * 2));
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }
            
            if (_hideUi)
                return;

            _shader.SetUniform("uOpacity", 0.75f);
            _shader.SetUniform("uModel",
                Matrix4.CreateFromQuaternion(_camera.PlaceCube.Rotation) *
                Matrix4.CreateTranslation(_camera.PlaceCube.Position));
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            
            SpriteBatch.Begin();
            SpriteBatch.Draw(_crosshair, new Vector2(SpriteBatch.Width, SpriteBatch.Height) / 2,
                Color.White, 0, _crosshair.Size.ToVector2() / 2, new Vector2(0.05f) * UiManager.UiScale);
            SpriteBatch.End();
            UiManager.Draw();
        }

        public override void Unload()
        {
            base.Unload();

            _texture.Dispose();
            _shader.Dispose();
            _crosshair.Dispose();
        }
    }
}