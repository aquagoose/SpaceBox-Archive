using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Image = OpenTK.Windowing.GraphicsLibraryFramework.Image;

namespace Cubic.Windowing
{
    public abstract unsafe class BaseGame : IDisposable
    {
        #region Private fields
        
        private WindowSettings _settings;
        private Window* _window;
        private bool _hasUnloaded;

        private double _secondsPerFrame;
        private uint _targetFps;
        
        private readonly GLFWCallbacks.MouseButtonCallback _mouseButtonCallback;
        private readonly GLFWCallbacks.KeyCallback _keyCallback;
        private readonly GLFWCallbacks.ScrollCallback _scrollCallback;
        private readonly GLFWCallbacks.CharCallback _charCallback;
        private readonly GLFWCallbacks.WindowSizeCallback _windowSizeCallback;
        
        #endregion

        #region Public fields

        public bool LockFps;

        #endregion

        #region Events
        
        public event OnResize Resize;
        public event OnCharInput CharInput;
        
        #endregion

        #region Properties
        
        public Size Size
        {
            get
            {
                GLFW.GetWindowSize(_window, out int width, out int height);
                return new Size(width, height);
            }
            set
            {
                GLFW.SetWindowSize(_window, value.Width, value.Height);
                VideoMode* mode = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());
                GLFW.SetWindowPos(_window, (int) (mode->Width / 2f - value.Width / 2f),
                    (int) (mode->Height / 2f - value.Height / 2f));
            }
        }

        public bool Fullscreen
        {
            get => GLFW.GetWindowMonitor(_window) != null;
            set
            {
                if (value)
                {
                    Monitor* primary = GLFW.GetPrimaryMonitor();
                    Size size = Size;
                    GLFW.SetWindowMonitor(_window, primary, 0, 0, size.Width, size.Height, GLFW.DontCare);
                }
                else
                {
                    // Get our size only once so we aren't calling the get method 4 times.
                    Size size = Size;
                    GLFW.SetWindowMonitor(_window, null, 0, 0, size.Width, size.Height, GLFW.DontCare);
                    VideoMode* mode = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());
                    // Set our window position separately so we can return the monitor to its original video mode before
                    // relocating the window, to ensure it is centered on the screen.
                    GLFW.SetWindowPos(_window, (int) (mode->Width / 2f - size.Width / 2f),
                        (int) (mode->Height / 2f - size.Height / 2f));
                }
            }
        }

        public uint TargetFps
        {
            get => _targetFps;
            set
            {
                _targetFps = value;
                _secondsPerFrame = 1d / value;
            }
        }

        #endregion

        public BaseGame(WindowSettings settings)
        {
            _settings = settings;
            _keyCallback = Input.KeyCallback;
            _mouseButtonCallback = Input.MouseCallback;
            _scrollCallback = Input.ScrollCallback;
            _charCallback = CharCallback;
            _windowSizeCallback = ResizeCallback;
        }
        
        #region Public methods

        public void Run(uint targetFps = default)
        {
            if (!GLFW.Init())
                throw new Exception("GLFW could not initialize.");

            if (targetFps != default)
            {
                TargetFps = targetFps;
                LockFps = true;
            }
            
            GLFW.WindowHint(WindowHintBool.Visible, false);
            GLFW.WindowHint(WindowHintBool.Resizable, false);
            GLFW.WindowHint(WindowHintInt.Samples, (int) _settings.SampleCount);
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

            _window = GLFW.CreateWindow(_settings.Size.Width, _settings.Size.Height, _settings.Title,
                _settings.StartFullscreen
                    ? _settings.StartingMonitor == default ? GLFW.GetPrimaryMonitor() : _settings.StartingMonitor
                    : null, null);

            if (_window == null)
            {
                GLFW.Terminate();
                throw new Exception("Window was not created.");
            }

            GLFW.SetWindowSizeCallback(_window, _windowSizeCallback);
            GLFW.SetKeyCallback(_window, _keyCallback);
            GLFW.SetCharCallback(_window, _charCallback);
            GLFW.SetMouseButtonCallback(_window, _mouseButtonCallback);
            GLFW.SetScrollCallback(_window, _scrollCallback);

            VideoMode* mode = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());
            GLFW.SetWindowPos(_window, (int) (mode->Width / 2f - _settings.Size.Width / 2f),
                (int) (mode->Height / 2f - _settings.Size.Height / 2f));

            List<Image> images = new List<Image>();

            foreach (OpenTK.Windowing.Common.Input.Image image in _settings.Icon.Images)
            {
                fixed (byte* p = image.Data)
                {
                    images.Add(new Image(image.Width, image.Height, p));
                }
            }
            
            GLFW.SetWindowIcon(_window, new ReadOnlySpan<Image>(images.ToArray()));
            
            GLFW.MakeContextCurrent(_window);
            GL.LoadBindings(new GLFWBindingsContext());
            
            Input.Update(_window);
            Time.Start();

            Initialize();
            
            // Manually call resize because why not.
            // Also fixes spacebox bug which I am too lazy to fix.
            Resize?.Invoke(Size);

            GLFW.ShowWindow(_window);

            while (!GLFW.WindowShouldClose(_window))
            {
                if (Time.ElapsedSecondsD - Time.PrevSecond < _secondsPerFrame && LockFps)
                    continue;
                
                GLFW.PollEvents();
                Input.Update(_window);
                Time.Update();

                Update();
                Draw();

                GLFW.SwapBuffers(_window);
            }

            Unload();
            GLFW.Terminate();
        }

        public void Close()
        {
            GLFW.SetWindowShouldClose(_window, true);
        }

        public virtual void Dispose()
        {
            if (!_hasUnloaded)
                Unload();
            GC.SuppressFinalize(this);
        }
        
        #endregion

        #region Callbacks
        
        private void ResizeCallback(Window* window, int width, int height) => Resize?.Invoke(new Size(width, height));

        private void CharCallback(Window* window, uint codepoint) => CharInput?.Invoke((char) codepoint);

        #endregion
        
        #region Virtual methods

        protected virtual void Initialize() { }

        protected virtual void Update() { }
        
        protected virtual void Draw() { }

        protected virtual void Unload()
        {
            _hasUnloaded = true;
        }
        
        #endregion

        public delegate void OnResize(Size size);

        public delegate void OnCharInput(char chr);
    }
}