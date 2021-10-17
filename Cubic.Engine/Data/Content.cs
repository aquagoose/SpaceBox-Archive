using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Cubic.Engine.Render;

namespace Cubic.Engine.Data
{
    public static class Content
    {
        public static string ContentDirectory { get; set; }
        public static bool Loaded { get; private set; }
        public static Dictionary<string, Bitmap[]> LoadedTextures = new Dictionary<string, Bitmap[]>();

        public static async void LoadContent(string entryPath)
        {
            LoadedTextures.Clear();
            Console.WriteLine("Loading textures...");
            Loaded = false;
            string[] files = Directory.GetFiles("Content/Textures", "*.ctf", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Console.WriteLine($"Loading {file}...");
                string key = Path.GetFileNameWithoutExtension(file);
                Bitmap[] bp = await Task.Run(() => Texture2D.LoadCTF(file));
                foreach (Bitmap b in bp)
                    b.RotateFlip(RotateFlipType.RotateNoneFlipY);
                LoadedTextures.Add(key, bp);
            }

            Loaded = true;
            Console.WriteLine("Loading done!");
        }
    }
}