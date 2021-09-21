using OpenTK.Mathematics;

namespace SpaceBox.Data
{
    public class SaveGame
    {
        public string WorldName { get; set; }
        
        public Vector3 PlayerPosition { get; set; }
        public Vector3 PlayerRotation { get; set; }
        
        public Vector3[] BlockPositions { get; set; }
    }
}