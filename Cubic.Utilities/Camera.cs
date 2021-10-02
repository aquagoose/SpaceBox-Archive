using System;
using OpenTK.Mathematics;

namespace Cubic.Utilities
{
    public class Camera
    {
        private float _fov;
        private float _aspectRatio;
        private float _near;
        private float _far;

        //private float _pitch;
        //private float _yaw;
        //private float _roll;
        
        private Vector3 _forward; 
        private Vector3 _right;
        private Vector3 _up;

        public Vector3 Forward => _forward;
        public Vector3 Right => _right;
        public Vector3 Up => _up;

        public Vector3 Position;

        /*public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                _pitch = MathHelper.DegreesToRadians(value);
                UpdateValues();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateValues();
            }
        }

        public float Roll
        {
            get => MathHelper.RadiansToDegrees(_roll);
            set
            {
                _roll = MathHelper.DegreesToRadians(value);
                UpdateValues();
            }
        }*/

        private Quaternion _rotation;
        
        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                UpdateValues();
            }
        }

        public Matrix4 ProjectionMatrix { get; private set; }

        public Matrix4 ViewMatrix
        {
            get
            {
                //Quaternion rot = Quaternion.FromEulerAngles(_pitch, _yaw, _roll);
                return Matrix4.LookAt(Position, Position + Forward, Up);
            }
        }
        
        public float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                _aspectRatio = value;
                GenerateProjectionMatrix();
            }
        }
        
        public float Near
        {
            get => _near;
            set
            {
                _near = value;
                GenerateProjectionMatrix();
            }
        }

        public float Far
        {
            get => _far;
            set
            {
                _far = value;
                GenerateProjectionMatrix();
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                _fov = MathHelper.DegreesToRadians(value);
                GenerateProjectionMatrix();
            }
        }

        public Camera(Vector3 position, Quaternion rotation, float aspectRatio, float fov, float near, float far)
        {
            Position = position;
            //_pitch = MathHelper.DegreesToRadians(rotation.Y);
            //_yaw = MathHelper.DegreesToRadians(rotation.X);
            //_roll = MathHelper.DegreesToRadians(rotation.Z);
            Rotation = rotation;
            UpdateValues();
            _aspectRatio = aspectRatio;
            _fov = MathHelper.DegreesToRadians(fov);
            _near = near;
            _far = far;
            GenerateProjectionMatrix();
        }

        private void GenerateProjectionMatrix()
        {
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _near, _far);
        }

        private void UpdateValues()
        {
            //Quaternion rot = Quaternion.FromEulerAngles(_pitch, -_yaw, _roll);
            //_forward = Vector3.Transform(Position, rot);
            //_right = Vector3.Normalize(Vector3.Cross(_forward, Vector3.UnitY));
            //_up = Vector3.Normalize(Vector3.Cross(_right, _forward));
            //Matrix4.CreateFromQuaternion()
            //_forward = rot * -Vector3.UnitZ;
            //_right = rot * Vector3.UnitX;
            //_up = rot * _right;
            
            //_forward.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            //_forward.Y = MathF.Sin(_pitch);
            //_forward.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _forward = Vector3.Normalize(Rotation * Vector3.UnitZ);
            _right = Vector3.Normalize(Rotation * Vector3.UnitX);
            _up = Vector3.Normalize(Rotation * Vector3.UnitY);

            //_right = Vector3.Normalize(Vector3.Cross(_forward, Vector3.UnitY));
            //_up = Vector3.Normalize(Vector3.Cross(_right, _forward));
        }
    }
}