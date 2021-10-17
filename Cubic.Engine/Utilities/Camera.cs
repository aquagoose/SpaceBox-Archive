using OpenTK.Mathematics;

namespace Cubic.Engine.Utilities
{
    public class Camera
    {
        private float _fov;
        private float _aspectRatio;
        private float _near;
        private float _far;

        private Vector3 _forward; 
        private Vector3 _right;
        private Vector3 _up;

        public Vector3 Forward => _forward;
        public Vector3 Right => _right;
        public Vector3 Up => _up;
        public Vector3 Back => -_forward;
        public Vector3 Left => -_right;
        public Vector3 Down => -_up;

        public Vector3 Position;
        
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

        public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Position + _forward, Up);

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
            _forward = Vector3.Normalize(Rotation * Vector3.UnitZ);
            _right = Vector3.Normalize(Rotation * Vector3.UnitX);
            _up = Vector3.Normalize(Rotation * Vector3.UnitY);
        }
    }
}