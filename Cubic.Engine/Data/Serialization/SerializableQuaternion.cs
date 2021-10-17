using System.Xml.Serialization;

namespace Cubic.Engine.Data.Serialization
{
    /// <summary>
    /// Represents a Serializable Quaternion. While regular Quaternions are indeed serializable, this class places the
    /// X, Y, Z, and W attributes in the XML element as attributes, instead of as separate tags, saving a bit of space.
    /// </summary>
    [XmlType("Quaternion")]
    public class SerializableQuaternion
    {
        [XmlAttribute]
        public float X { get; set; }
        
        [XmlAttribute]
        public float Y { get; set; }
        
        [XmlAttribute]
        public float Z { get; set; }
        
        [XmlAttribute]
        public float W { get; set; }
    }
}