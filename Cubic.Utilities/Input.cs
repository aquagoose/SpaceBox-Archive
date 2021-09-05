using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubic.Utilities
{
    public static class Input
    {
        private static KeyboardState _keyboardState;
        private static MouseState _mouseState;

        public static KeyboardState KeyboardState => _keyboardState;
        public static MouseState MouseState => _mouseState;

        public static bool IsKeyDown(Keys key) => _keyboardState.IsKeyDown(key);

        public static bool IsKeyPressed(Keys key) => _keyboardState.IsKeyPressed(key);

        public static bool IsMouseButtonDown(MouseButton button) => _mouseState.IsButtonDown(button);

        public static bool IsMouseButtonPressed(MouseButton button) =>
            _mouseState.IsButtonDown(button) && !_mouseState.WasButtonDown(button);

        public static Vector2 MousePosition => _mouseState.Position;

        public static Vector2 MouseScroll => _mouseState.Scroll;

        /// <summary>
        /// Update the input manager. <b>You should only call this ONCE in your application per-frame.</b>
        /// </summary>
        /// <param name="keyboardState">The keyboard state.</param>
        /// <param name="mouseState">The mouse state.</param>
        public static void Update(KeyboardState keyboardState, MouseState mouseState)
        {
            _keyboardState = keyboardState;
            _mouseState = mouseState;
        }
    }
}