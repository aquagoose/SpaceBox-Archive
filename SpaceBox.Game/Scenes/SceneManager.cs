using Cubic.Engine.GUI;
using Cubic.Engine.Render;

namespace Spacebox.Game.Scenes
{
    public static class SceneManager
    {
        private static Scene _currentScene = null;
        private static Scene _sceneToChange = null;

        private static SpriteBatch _spriteBatch;
        private static UIManager _uiManager;

        public static void Initialize(SpaceboxGame game, Scene startingScene, SpriteBatch spriteBatch, UIManager uiManager)
        {
            _spriteBatch = spriteBatch;
            _uiManager = uiManager;

            _currentScene = startingScene;
            _currentScene.SpriteBatch = _spriteBatch;
            _currentScene.UiManager = uiManager;
            
            _currentScene.Initialize(game);
        }
        
        public static void Update(SpaceboxGame game)
        {
            if (_sceneToChange != null)
            {
                _currentScene.Dispose();
                _uiManager.Clear();
                _currentScene = _sceneToChange;
                _sceneToChange = null;
                _currentScene.SpriteBatch = _spriteBatch;
                _currentScene.UiManager = _uiManager;
                _currentScene.Initialize(game);
            }
            
            _currentScene.Update(game);
        }

        public static void Draw(SpaceboxGame game)
        {
            _currentScene.Draw(game);
        }

        public static void SetScene(Scene scene)
        {
            _sceneToChange = scene;
        }

        public static void DisposeScenes()
        {
            _currentScene.Dispose();
        }
    }
}