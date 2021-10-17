using Cubic.Engine.Data;
using Cubic.Engine.Render;
using Cubic.Engine.Scenes;
using Cubic.Engine.Windowing;

namespace Cubic.Engine
{
    public class CubicGame : BaseGame
    {
        private Scene _openingScene;
        
        public ImGuiRenderer ImGuiRenderer { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        
        public CubicGame(WindowSettings settings, Scene openingScene, string contentDirectory = "Content") : base(settings)
        {
            Content.ContentDirectory = contentDirectory;
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            ImGuiRenderer = new ImGuiRenderer(this);
            
            //SceneManager.Initialize(_openingScene, );
        }

        protected override void Update()
        {
            base.Update();
            
            ImGuiRenderer.Update(Time.DeltaTime);
            
            SceneManager.Update();
        }

        protected override void Draw()
        {
            base.Draw();
            
            SceneManager.Draw();
            
            ImGuiRenderer.Render();
        }
    }
}