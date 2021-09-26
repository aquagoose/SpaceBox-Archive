using OpenTK.Mathematics;

namespace SpaceBox.Sandbox.Grids
{
    /// <summary>
    /// Represents a helper class that aids in generating grids.
    /// </summary>
    public class PlaceCube
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
    }
}