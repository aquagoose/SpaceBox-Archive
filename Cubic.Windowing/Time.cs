using System;
using System.Diagnostics;

namespace Cubic.Windowing
{
    public static class Time
    {
        private static Stopwatch _sw;
        internal static double PrevSecond;
        private static long _storeMs;
        private static ulong _framesCount;

        /// <summary>
        /// The amount of time in seconds that has passed since the previous frame, using single precision.
        /// </summary>
        public static float DeltaTime => (float) DeltaTimeD;
        /// <summary>
        /// The amount of time in seconds that has passed since the previous frame, using double precision.
        /// </summary>
        public static double DeltaTimeD { get; private set; }
        /// <summary>
        /// The total number of elapsed seconds that has passed since the application opened, using single precision.
        /// </summary>
        public static float ElapsedSeconds => (float) _sw.Elapsed.TotalSeconds;
        /// <summary>
        /// The total number of elapsed seconds that has passed since the application opened, using double precision.
        /// </summary>
        public static double ElapsedSecondsD => _sw.Elapsed.TotalSeconds;
        /// <summary>
        /// The total number of elapsed milliseconds that have passed since the application opened.
        /// </summary>
        public static long ElapsedMilliseconds => _sw.ElapsedMilliseconds;
        
        /// <summary>
        /// The total number of frames that have been processed since the application opened.
        /// </summary>
        public static ulong TotalFrames { get; private set; }
        /// <summary>
        /// The current Frames Per Second. This updates once per second.
        /// </summary>
        public static int Fps { get; private set; }

        /// <summary>
        /// Start the Time system. <b>This must only be called ONCE in an application.</b>
        /// </summary>
        internal static void Start()
        {
            _sw = Stopwatch.StartNew();
            PrevSecond = _sw.Elapsed.TotalSeconds;
            _storeMs = _sw.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// Update the Time system. <b>This must only be called ONCE per frame.</b>
        /// </summary>
        internal static void Update()
        {
            TotalFrames++;
            _framesCount++;
            if (_sw.ElapsedMilliseconds - _storeMs >= 1000)
            {
                Fps = (int) _framesCount;
                _framesCount = 0;
                _storeMs = _sw.ElapsedMilliseconds;
            }
            
            DeltaTimeD = _sw.Elapsed.TotalSeconds - PrevSecond;
            PrevSecond = _sw.Elapsed.TotalSeconds;
        }
    }
}