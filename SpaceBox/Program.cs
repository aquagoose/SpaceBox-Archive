using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Cubic.Engine.Render;
using Cubic.Engine.Render.Shaders;
using Cubic.Engine.Windowing;
using Cubic.Forms;
using Eto.Forms;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceBox.Data;
using Spacebox.Game;
using Image = OpenTK.Windowing.Common.Input.Image;

namespace Spacebox
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Bitmap icon = new Bitmap("Content/Textures/Images/Icon.bmp");
            Bitmap icon = Texture2D.LoadCTF("Content/Textures/Images/icon.ctf")[0];

            bool createConfig = false;
            
            SpaceboxConfig config = Data.GetSpaceBoxConfig("spacebox.cfg");
            if (config == null)
            {
                Console.WriteLine("SpaceBox config is missing! Creating new config.");
                config = new SpaceboxConfig();
                Data.SaveSpaceBoxConfig(config, "spacebox.cfg");
                createConfig = true;
            }

            WindowSettings windowSettings = new WindowSettings()
            {
                Size = config.Display.Resolution,
                Title = "SpaceBox",
                StartFullscreen = config.Display.Fullscreen,
                SampleCount = 32,
                Icon = new WindowIcon(new Image(icon.Size.Width, icon.Size.Height, icon.Data)),
            };

#if DEBUG
            using (SpaceboxGame game = new SpaceboxGame(windowSettings, config, createConfig))
                game.Run();
#else
            try
            {
                using (SpaceboxGame game = new SpaceboxGame(windowSettings, config, createConfig))
                    game.Run();
            }
            catch (Exception e)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    new Application(Eto.Platforms.WinForms).Run(new CrashForm(e));
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    new Application(Eto.Platforms.Gtk).Run(new CrashForm(e));
            }
#endif
        }
    }
}