using System.Xml.Serialization;
using Cubic.Engine.Data.Serialization;

namespace SpaceBox.Data.Serialization
{
    [XmlType("Block")]
    public class SerializableBlock
    {
        public string Name { get; set; }
        public SerializableVector3 Coord { get; set; }
    }
}