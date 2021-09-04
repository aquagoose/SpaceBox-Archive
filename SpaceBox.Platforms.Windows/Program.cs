using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;

namespace Spacebox.Platforms.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = GameWindowSettings.Default;

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(1280, 720),
                Title = "SpaceBox",
                StartVisible = false,
                NumberOfSamples = 8, // 8x MSAA
                //WindowBorder = WindowBorder.Fixed // Uncomment as necessary
            };

            using (SpaceboxGame game = new SpaceboxGame(gameWindowSettings, nativeWindowSettings))
                game.Run();
        }
    }
}