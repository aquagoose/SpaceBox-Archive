using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using Image = OpenTK.Windowing.Common.Input.Image;

namespace Spacebox.Platforms.Windows
{
    class Program
    {
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

            GameWindowSettings gameWindowSettings = GameWindowSettings.Default;

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(1280, 720),
                Title = "SpaceBox",
                StartVisible = false,
                NumberOfSamples = 8, // 8x MSAA
                //WindowBorder = WindowBorder.Fixed // Uncomment as necessary
                Icon = new WindowIcon(new Image(icon.Width, icon.Height, image))
            };
            
            icon.Dispose();

            using (SpaceboxGame game = new SpaceboxGame(gameWindowSettings, nativeWindowSettings))
                game.Run();
        }
    }
}