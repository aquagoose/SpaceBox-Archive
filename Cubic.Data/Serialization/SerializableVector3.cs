using System.Xml.Serialization;

namespace Cubic.Data.Serialization
{
    /// <summary>
    /// Represents a Serializable Vector3. While regular Vector3s are indeed serializable, this class places the X, Y,
    /// and Z attributes in the XML element as attributes, instead of as separate tags, saving a bit of space.
    /// </summary>
    [XmlType("Vector3")]
    public class SerializableVector3
    {
        [XmlAttribute]
        public float X { get; set; }
        
        [XmlAttribute]
        public float Y { get; set; }
        
        [XmlAttribute]
        public float Z { get; set; }
    }
}