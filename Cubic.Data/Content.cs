using System;
using System.Collections.Generic;
using System.IO;
using Cubic.Render;

namespace Cubic.Data
{
    public static class Content
    {
        public static Dictionary<string, Texture2D> LoadedTextures = new Dictionary<string, Texture2D>();

        public static void LoadAllTextures(string entryPath)
        {
            string[] files = Directory.GetFiles(entryPath, "*.ctf", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Console.WriteLine($"Loading {file}...");
                string key = Path.GetFileNameWithoutExtension(file);
                Texture2D value = new Texture2D(file);
            }
        }
    }
}