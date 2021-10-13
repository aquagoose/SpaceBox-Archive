using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Cubic.Render;

namespace Cubic.Data
{
    public static class Content
    {
        public static bool Loaded { get; private set; }
        public static Dictionary<string, Bitmap[]> LoadedTextures = new Dictionary<string, Bitmap[]>();

        public static async void LoadAllTextures(string entryPath)
        {
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