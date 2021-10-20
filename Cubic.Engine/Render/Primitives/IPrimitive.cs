namespace Cubic.Engine.Render.Primitives
{
    public interface IPrimitive
    {
        public float[] Vertices { get; }
        public uint[] Indices { get; }
    }
}