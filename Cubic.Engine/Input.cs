using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using IA = OpenTK.Windowing.GraphicsLibraryFramework.InputAction;

namespace Cubic.Engine
{
    public static unsafe class Input
    {
        private static Dictionary<string, InputAction> _inputActions = new Dictionary<string, InputAction>();

        private static HashSet<string> _actionsPressed = new HashSet<string>();

        private static List<KeyState> _keysQueue = new List<KeyState>();
        private static List<MouseState> _mouseQueue = new List<MouseState>();

        private static HashSet<Keys> _keysHeld = new HashSet<Keys>();
        private static HashSet<Keys> _newKeysThisFrame = new HashSet<Keys>();
        
        private static HashSet<MouseButton> _mouseButtonsHeld = new HashSet<MouseButton>();
        private static HashSet<MouseButton> _newMouseButtonsThisFrame = new HashSet<MouseButton>();

        private static Vector2 _lastMousePos;
        private static Vector2 _lastScroll;

        private static Window* _window;

        /// <summary>
        /// Returns true the entire time the given key is held.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is held.</returns>
        public static bool IsKeyDown(Keys key) => _keysHeld.Contains(key);

        /// <summary>
        /// Returns true for only the first frame that the given key is pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was pressed in the current frame.</returns>
        public static bool IsKeyPressed(Keys key) => _newKeysThisFrame.Contains(key);

        /// <summary>
        /// Returns true if any key on the keyboard is pressed or held.
        /// </summary>
        /// <returns>True if any key is held.</returns>
        public static bool IsAnyKeyDown() => _keysHeld.Count > 0;

        /// <summary>
        /// Returns true the entire time the given mouse button is held down.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the button is held down.</returns>
        public static bool IsMouseButtonDown(MouseButton button) => _mouseButtonsHeld.Contains(button);

        /// <summary>
        /// Returns true only for the first frame that the given mouse button is pressed.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the button was pressed in the current frame.</returns>
        public static bool IsMouseButtonPressed(MouseButton button) => _newMouseButtonsThisFrame.Contains(button);

        /// <summary>
        /// Returns true the entire time the given action's buttons/keys are held down.
        /// </summary>
        /// <param name="actionName">The name of the action.</param>
        /// <returns>True if the given action's condition is met.</returns>
        public static bool IsActionHeld(string actionName) => _actionsPressed.Contains(actionName);

        /// <summary>
        /// The current position of the mouse relative to the window.
        /// </summary>
        public static Vector2 MousePosition { get; private set; }

        /// <summary>
        /// The current position of the scroll wheel.
        /// </summary>
        public static Vector2 MouseScroll { get; private set; }
        
        /// <summary>
        /// The amount the scroll wheel has changed since the last frame.
        /// </summary>
        public static Vector2 ScrollDelta { get; private set; }

        /// <summary>
        /// The amount the mouse position has changed since the last frame.
        /// </summary>
        public static Vector2 MouseDelta { get; private set; }

        /// <summary>
        /// Set the window's cursor state.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid enum value is given.</exception>
        public static CursorState CursorState
        {
            get
            {
                CursorModeValue cursorValue = GLFW.GetInputMode(_window, CursorStateAttribute.Cursor);
                switch (cursorValue)
                {
                    case CursorModeValue.CursorNormal:
                        return CursorState.Visible;
                    case CursorModeValue.CursorHidden:
                        return CursorState.Hidden;
                    case CursorModeValue.CursorDisabled:
                        return CursorState.Captured;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                CursorModeValue cursorValue = value switch
                {
                    CursorState.Visible => CursorModeValue.CursorNormal,
                    CursorState.Hidden => CursorModeValue.CursorHidden,
                    CursorState.Captured => CursorModeValue.CursorDisabled,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                };

                GLFW.SetInputMode(_window, CursorStateAttribute.Cursor, cursorValue);
            }
        }

        public static void RegisterAction(string actionName, InputAction action)
        {
            _inputActions.Add(actionName, action);
        }
        
        /// <summary>
        /// Update the current input. This should be called immediately after calling GLFW.PollEvents().
        /// </summary>
        /// <param name="window">The active window.</param>
        internal static void Update(Window* window)
        {
            _window = window;
            _newKeysThisFrame.Clear();
            _newMouseButtonsThisFrame.Clear();
            
            foreach (KeyState keyState in _keysQueue)
            {
                if (keyState.Pressed)
                {
                    if (_keysHeld.Add(keyState.Key))
                        _newKeysThisFrame.Add(keyState.Key);
                }
                else
                {
                    _keysHeld.Remove(keyState.Key);
                    _newKeysThisFrame.Remove(keyState.Key);
                }
            }
            _keysQueue.Clear();
            
            foreach (MouseState mouseState in _mouseQueue)
            {
                if (mouseState.Pressed)
                {
                    if (_mouseButtonsHeld.Add(mouseState.Button))
                        _newMouseButtonsThisFrame.Add(mouseState.Button);
                }
                else
                {
                    _mouseButtonsHeld.Remove(mouseState.Button);
                    _newMouseButtonsThisFrame.Remove(mouseState.Button);
                }
            }
            _mouseQueue.Clear();

            if (IsAnyKeyDown())
            {
                foreach (KeyValuePair<string, InputAction> action in _inputActions)
                {
                    foreach (Keys key in action.Value.ActionKeys)
                    {
                        if (IsKeyDown(key))
                            _actionsPressed.Add(action.Key);
                        else
                            _actionsPressed.Remove(action.Key);
                    }
                }
            }
            else
                _actionsPressed.Clear();
            
            GLFW.GetCursorPos(window, out double xPos, out double yPos);
            MousePosition = new Vector2((float) xPos, (float) yPos);
            MouseDelta = MousePosition - _lastMousePos;

            ScrollDelta = MouseScroll - _lastScroll;

            _lastMousePos = MousePosition;
            _lastScroll = MouseScroll;
        }

        internal static void KeyCallback(Window* window, Keys key, int scanCode, IA action, KeyModifiers mods)
        {
            if (action != IA.Repeat)
                _keysQueue.Add(new KeyState(key, action == IA.Press));
        }

        internal static void MouseCallback(Window* window, MouseButton button, IA action, KeyModifiers mods)
        {
            if (action != IA.Repeat)
                _mouseQueue.Add(new MouseState(button, action == IA.Press));
        }

        internal static void ScrollCallback(Window* window, double offsetX, double offsetY)
        {
            MouseScroll += new Vector2((float) offsetX, (float) offsetY);
        }

        private struct KeyState
        {
            public Keys Key { get; }
            public bool Pressed { get; }

            public KeyState(Keys key, bool pressed)
            {
                Key = key;
                Pressed = pressed;
            }
        }

        private struct MouseState
        {
            public MouseButton Button { get; }
            public bool Pressed { get; }

            public MouseState(MouseButton button, bool pressed)
            {
                Button = button;
                Pressed = pressed;
            }
        }
    }

    public enum CursorState
    {
        /// <summary>
        /// The mouse cursor is fully visible and not confined to the window.
        /// </summary>
        Visible,
        /// <summary>
        /// The mouse cursor is hidden, however not confined to the window.
        /// </summary>
        Hidden,
        /// <summary>
        /// The mouse cursor is hidden, and locked to the center of the screen.
        /// </summary>
        Captured
    }
}