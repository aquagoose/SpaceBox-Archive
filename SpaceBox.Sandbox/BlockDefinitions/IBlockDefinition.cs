using SpaceBox.Sandbox.Grids;

namespace SpaceBox.Sandbox.BlockDefinitions
{
    public interface IBlockDefinition
    {
        public float Vertices { get; }
        public uint Indices { get; }
        
        public GridSize BlockSize { get; }
    }

    public enum BlockSize
    {
        All,
        Small,
        Medium,
        Large
    }
}