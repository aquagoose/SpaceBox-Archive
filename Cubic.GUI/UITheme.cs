using System.Drawing;

namespace Cubic.GUI
{
    public class UITheme
    {
        public Color BackColor { get; set; }
        public Color HoverColor { get; set; }
        public Color ClickColor { get; set; }
        public Color BorderColor { get; set; }
        public int BorderWidth { get; set; }
        
        public Color TextColor { get; set; }
        public int TextSize { get; set; }
        
        public bool ButtonExecuteOnRelease { get; set; }
        
        public string DefaultFontPath { get; set; }

        public UITheme()
        {
            BackColor = Color.Gray;
            HoverColor = Color.DarkGray;
            ClickColor = Color.Gray;
            BorderColor = Color.LightGray;

            BorderWidth = 2;
            
            TextColor = Color.White;
            TextSize = 32;
            ButtonExecuteOnRelease = true;

            DefaultFontPath = "Content/Fonts/arial.ttf";
        }
    }
}