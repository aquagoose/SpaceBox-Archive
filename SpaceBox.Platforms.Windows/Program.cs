using System;
using System.Drawing;
using Cubic.Forms;
using Cubic.Render;
using Cubic.Windowing;
using Eto.Forms;
using OpenTK.Windowing.Common.Input;
using SpaceBox.Data;
using Spacebox.Game;
using Image = OpenTK.Windowing.Common.Input.Image;

namespace Spacebox.Platforms.Windows
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Bitmap icon = new Bitmap("Content/Textures/Images/Icon.bmp");
            Bitmap icon = Texture2D.LoadCTF("Content/Textures/Images/Icon.ctf")[0];
            byte[] image = new byte[icon.Width * icon.Height * 4];
            for (int x = 0; x < icon.Width; x++)
            {
                for (int y = 0; y < icon.Height; y++)
                {
                    Color color = icon.GetPixel(x, y);
                    image[(y * icon.Width + x) * 4] = color.R;
                    image[((y * icon.Width + x) * 4) + 1] = color.G;
                    image[((y * icon.Width + x) * 4) + 2] = color.B;
                    image[((y * icon.Width + x) * 4) + 3] = color.A;
                }
            }

            SpaceboxConfig config = Data.GetSpaceBoxConfig("spacebox.cfg");
            if (config == null)
            {
                Console.WriteLine("SpaceBox config is missing! Creating new config.");
                config = new SpaceboxConfig();
                Data.SaveSpaceBoxConfig(config, "spacebox.cfg");
            }

            WindowSettings windowSettings = new WindowSettings()
            {
                Size = config.Display.Resolution,
                Title = "SpaceBox",
                StartFullscreen = config.Display.Fullscreen,
                SampleCount = 32,
                Icon = new WindowIcon(new Image(icon.Width, icon.Height, image)),
            };
            
            icon.Dispose();

#if DEBUG
            using (SpaceboxGame game = new SpaceboxGame(windowSettings, config))
                game.Run();
#else
            try
            {
                using (SpaceboxGame game = new SpaceboxGame(windowSettings, config))
                    game.Run();
            }
            catch (Exception e)
            {
                new Application(Eto.Platforms.WinForms).Run(new CrashForm(e));
            }
#endif
        }
    }
}