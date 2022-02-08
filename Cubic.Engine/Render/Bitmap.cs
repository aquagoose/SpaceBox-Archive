using System.Drawing;
using System.IO;
using StbImageSharp;

namespace Cubic.Engine.Render;

public class Bitmap
{
    public readonly byte[] Data;
    public readonly Size Size;

    public Bitmap(string path)
    {
        using Stream stream = File.OpenRead(path);
        ImageResult result = ImageResult.FromStream(stream);
        Data = result.Data;
        Size = new Size(result.Width, result.Height);
    }

    public Bitmap(Bitmap bitmap)
    {
        Data = bitmap.Data;
        Size = bitmap.Size;
    }

    public Bitmap(int width, int height, byte[] data)
    {
        Data = data;
        Size = new Size(width, height);
    }
    
    
}