using System.Diagnostics;

namespace Cubic.Utilities
{
    public static class Time
    {
        private static Stopwatch _sw;
        private static long _prevMs;

        public static float DeltaTime { get; private set; }
        public static float ElapsedSeconds => _sw.ElapsedMilliseconds / 1000f;
        public static long ElapsedMilliseconds => _sw.ElapsedMilliseconds;

        /// <summary>
        /// Start the Time system. <b>This must only be called ONCE in an application.</b>
        /// </summary>
        public static void Start()
        {
            _sw = Stopwatch.StartNew();
            _prevMs = _sw.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// Update the Time system. <b>This must only be called ONCE per frame.</b>
        /// </summary>
        public static void Update()
        {
            DeltaTime = (_sw.ElapsedMilliseconds - _prevMs) / 1000f;
            _prevMs = _sw.ElapsedMilliseconds;
        }
    }
}