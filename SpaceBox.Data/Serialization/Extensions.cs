using Cubic.Engine.Data.Serialization;
using OpenTK.Mathematics;

namespace SpaceBox.Data.Serialization
{
    public static class Extensions
    {
        public static SerializableVector3 ToSerializable(this Vector3 vector3)
        {
            return new SerializableVector3()
            {
                X = vector3.X,
                Y = vector3.Y,
                Z = vector3.Z
            };
        }

        public static SerializableQuaternion ToSerializable(this Quaternion quaternion)
        {
            return new SerializableQuaternion()
            {
                X = quaternion.X,
                Y = quaternion.Y,
                Z = quaternion.Z,
                W = quaternion.W
            };
        }

        public static Vector3 ToNormal(this SerializableVector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static Quaternion ToNormal(this SerializableQuaternion quat)
        {
            return new Quaternion(quat.X, quat.Y, quat.Z, quat.W);
        }
    }
}