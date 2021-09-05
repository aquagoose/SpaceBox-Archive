using System.Drawing;
using Cubic.Utilities;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubic.GUI
{
    public class Button : UIElement
    {
        private FillRectangle _rectangle;
        private BorderRectangle _border;
        
        public Button(UIManager manager, Position position, Size size) : base(manager, position, size,
            Color.White)
        {
            _rectangle = new FillRectangle(manager, position, size, manager.Theme.BackColor);
            _border = new BorderRectangle(manager, position, size, manager.Theme.BorderWidth,
                manager.Theme.BorderColor);
        }

        protected internal override void Update(ref bool mouseTaken)
        {
            base.Update(ref mouseTaken);

            if (Hovering)
            {
                _rectangle.Color = UiManager.Theme.HoverColor;
                if (Input.IsMouseButtonDown(MouseButton.Left))
                    _rectangle.Color = UiManager.Theme.ClickColor;
            }
            else
                _rectangle.Color = UiManager.Theme.BackColor;
            
            _rectangle.Update(ref mouseTaken);
            _border.Update(ref mouseTaken);
        }

        protected internal override void Draw()
        {
            _rectangle.Draw();
            _border.Draw();
        }
    }
}