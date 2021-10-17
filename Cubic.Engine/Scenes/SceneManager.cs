using Cubic.Engine.GUI;
using Cubic.Engine.Render;

namespace Cubic.Engine.Scenes
{
    public static class SceneManager
    {
        private static Scene _currentScene = null;
        private static Scene _sceneToChange = null;

        private static SpriteBatch _spriteBatch;
        private static UIManager _uiManager;

        internal static void Initialize(Scene startingScene, SpriteBatch spriteBatch, UIManager uiManager)
        {
            _spriteBatch = spriteBatch;
            _uiManager = uiManager;

            _currentScene = startingScene;
            _currentScene.SpriteBatch = _spriteBatch;
            _currentScene.UiManager = uiManager;

            _currentScene.Initialize();
        }

        internal static void Update()
        {
            if (_sceneToChange != null)
            {
                _currentScene.Dispose();
                _uiManager.Clear();
                _currentScene = _sceneToChange;
                _sceneToChange = null;
                _currentScene.SpriteBatch = _spriteBatch;
                _currentScene.UiManager = _uiManager;
                _currentScene.Initialize();
            }

            _currentScene.Update();
        }

        internal static void Draw()
        {
            _currentScene.Draw();
        }

        public static void SetScene(Scene scene)
        {
            _sceneToChange = scene;
        }

        internal static void DisposeScenes()
        {
            _currentScene.Dispose();
        }
    }
}