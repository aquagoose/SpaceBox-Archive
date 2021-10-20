namespace Cubic.Engine.Render.Primitives
{
    public class Quad : IPrimitive
    {
        public float[] Vertices { get; }
        public uint[] Indices { get; }
    }
}