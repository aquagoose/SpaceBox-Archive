using System.Xml.Serialization;
using Cubic.Data.Serialization;
using SpaceBox.Sandbox.Grids;

namespace SpaceBox.Data.Serialization
{
    [XmlType("Grid")]
    public class SerializableGrid
    {
        public string Name { get; set; }
        public SerializableVector3 Position { get; set; }
        public SerializableQuaternion Orientation { get; set; }
        
        public GridType GridType { get; set; }
        public GridSize GridSize { get; set; }
        
        public SerializableBlock[] Blocks { get; set; }
    }
}