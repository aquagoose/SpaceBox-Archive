using System;
using System.Drawing;
using Cubic.Forms;
using Eto.Forms;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using SpaceBox.Data;
using Image = OpenTK.Windowing.Common.Input.Image;
using WindowState = OpenTK.Windowing.Common.WindowState;

namespace Spacebox.Platforms.Windows
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Bitmap icon = new Bitmap("Content/Textures/Images/Icon.bmp");
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

            GameWindowSettings gameWindowSettings = GameWindowSettings.Default;

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(config.Display.Resolution.Width, config.Display.Resolution.Height),
                Title = "SpaceBox",
                StartVisible = config.Display.Fullscreen,
                NumberOfSamples = 32, // 8x MSAA
                WindowBorder = WindowBorder.Fixed, // Uncomment as necessary
                WindowState = config.Display.Fullscreen ? WindowState.Fullscreen : WindowState.Normal,
                Icon = new WindowIcon(new Image(icon.Width, icon.Height, image))
            };
            
            icon.Dispose();

            //try
            //{
                using (SpaceboxGame game = new SpaceboxGame(gameWindowSettings, nativeWindowSettings, config))
                    game.Run();
            //}
            //catch (Exception e)
            //{
            //    new Application(Eto.Platforms.WinForms).Run(new CrashForm(e));
            //}
        }
    }
}