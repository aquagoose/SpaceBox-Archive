using System.Drawing;
using Cubic.Utilities;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Font = Cubic.GUI.Fonts.Font;

namespace Cubic.GUI
{
    public class Button : UIElement
    {
        public event ButtonOnClick OnClick;
        
        private FillRectangle _rectangle;
        private BorderRectangle _border;
        private Font _font;
        private bool _isClicked;
        
        public int FontSize { get; set; }
        public string Text { get; set; }
        
        public bool ExecuteOnRelease { get; set; }

        public Button(UIManager manager, Position position, Size size, string text = "", string fontPath = null,
            int fontSize = default) : base(manager, position, size, Color.White)
        {
            _rectangle = new FillRectangle(manager, position, size, manager.Theme.BackColor);
            _border = new BorderRectangle(manager, position, size, manager.Theme.BorderWidth,
                manager.Theme.BorderColor);
            ExecuteOnRelease = manager.Theme.ButtonExecuteOnRelease;
            
            _font = new Font(fontPath ?? UiManager.Theme.DefaultFontPath, SpriteBatch);
            Text = text;
            FontSize = fontSize == default ? UiManager.Theme.TextSize : fontSize;
        }

        protected internal override void Update(ref bool mouseTaken)
        {
            base.Update(ref mouseTaken);

            if (Hovering)
            {
                _rectangle.Color = UiManager.Theme.HoverColor;
                if (Input.IsMouseButtonDown(MouseButton.Left))
                {
                    _rectangle.Color = UiManager.Theme.ClickColor;
                    if (!_isClicked)
                    {
                        _isClicked = true;
                        if (!ExecuteOnRelease)
                            OnClick?.Invoke();
                    }
                }
                else if (_isClicked && ExecuteOnRelease)
                {
                    _isClicked = false;
                    OnClick?.Invoke();
                }
                else
                    _isClicked = false;
            }
            else
            {
                _rectangle.Color = UiManager.Theme.BackColor;
                _isClicked = false;
            }

            _rectangle.Update(ref mouseTaken);
            _border.Update(ref mouseTaken);
        }

        protected internal override void Draw()
        {
            _rectangle.Draw();
            _border.Draw();
            _font?.DrawString((uint) FontSize, Text,
                Position.ScreenPosition + Size.ToVector2() / 2f - _font.MeasureString((uint) FontSize, Text) / 2f,
                Vector2.One, Color.White);
        }

        public delegate void ButtonOnClick();
    }
}