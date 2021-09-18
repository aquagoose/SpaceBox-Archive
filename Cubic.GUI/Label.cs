using System.Drawing;
using OpenTK.Mathematics;
using Font = Cubic.GUI.Fonts.Font;

namespace Cubic.GUI
{
    public class Label : UIElement
    {
        private Font _font;
        
        public string Text { get; set; }
        public int FontSize { get; set; }
        //public string FontPath { get; set; }
        
        public Label(UIManager manager, Position position, string text = "", string fontPath = null, int fontSize = default, Color color = default) : 
            base(manager, position, new Size(0, 0), Color.White)
        {
            Color = color == default ? UiManager.Theme.TextColor : color;
            _font = new Font(fontPath ?? UiManager.Theme.DefaultFontPath, SpriteBatch);
            Text = text;
            FontSize = fontSize == default ? UiManager.Theme.TextSize : fontSize;
        }

        protected internal override void Draw()
        {
            _font.DrawString((uint) (FontSize * UiManager.UiScale.X), Text, Position.ScreenPosition, Vector2.One, Color);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _font.Dispose();
        }
    }
}