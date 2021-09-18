using System;
using System.Drawing;
using Cubic.Render;
using Cubic.Utilities;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubic.GUI
{
    /// <summary>
    /// Represents the base UI element, including the major logic to make sure each UI element interacts with each other correctly.
    /// </summary>
    public abstract class UIElement : IDisposable
    {
        protected UIManager UiManager { get; }
        
        /// <summary>
        /// The Position of the element.
        /// </summary>
        public Position Position { get; set; }
        /// <summary>
        /// The element's size.
        /// </summary>
        public Size Size { get; set; }
        /// <summary>
        /// The element's colour, if any.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// The element's rotation.
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// The element's origin point. By default this is (0, 0) (top left).
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// If true, this element will not "take" the mouse.
        /// </summary>
        public bool MouseTransparent { get; set; } = false;

        /// <summary>
        /// If false, the element will not be rendered, and will not "take" the mouse.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// If true, the element will still render, however will not respond to keyboard or mouse input.
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// If true, the element will ignore all scaling by the UI.
        /// </summary>
        public bool IgnoreScale { get; set; } = false;

        /// <summary>
        /// If true, this element was the one that was most recently clicked on. Items such as text boxes will use this extensively.
        /// </summary>
        public bool Focused { get; set; } = false;

        /// <summary>
        /// If true, the mouse cursor is hovering over this element, and it has not been "taken" by any other element.
        /// </summary>
        public bool Hovering { get; set; } = false;

        protected SpriteBatch SpriteBatch { get; }

        public UIElement(UIManager manager, Position position, Size size, Color color)
        {
            UiManager = manager;
            Position = position;
            Size = size;
            Color = color;
            SpriteBatch = manager.SpriteBatch;
            Position.Update(UiManager);
        }
        
        /// <summary>
        /// Update the UI element.
        /// <b>IMPORTANT:</b> Place the "base.Update()" call <b>AT THE TOP</b> of your override, otherwise the mouse catching won't work properly.
        /// </summary>
        /// <param name="mouseTaken">Whether or not this element "takes" the mouse.</param>
        protected internal virtual void Update(ref bool mouseTaken)
        {
            if (!Disabled && !MouseTransparent)
            {
                Vector2 scale = IgnoreScale ? Vector2.One : UiManager.UiScale;
                if (Input.MousePosition.X >= Position.X && Input.MousePosition.X <= Position.X + Size.Width * scale.X &&
                    Input.MousePosition.Y >= Position.Y && Input.MousePosition.Y <= Position.Y + Size.Height * scale.Y &&
                    !mouseTaken)
                {
                    mouseTaken = true;
                    Hovering = true;

                    if (Input.IsMouseButtonDown(MouseButton.Left))
                        Focused = true;
                }
                else
                {
                    Hovering = false;
                    if (Input.IsMouseButtonDown(MouseButton.Left))
                        Focused = false;
                }
            }
            
            Position.Update(UiManager);
        }

        protected internal abstract void Draw();
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}