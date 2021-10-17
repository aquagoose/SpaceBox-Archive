using Cubic.Engine.Data.Serialization;
using OpenTK.Mathematics;
using SpaceBox.Data.Serialization;

namespace SpaceBox.Data
{
    public class SaveGame
    {
        public string WorldName { get; set; }
        
        public SerializableVector3 PlayerPosition { get; set; }
        public SerializableQuaternion PlayerRotation { get; set; }
        
        public SerializableGrid[] Grids { get; set; }
    }
}