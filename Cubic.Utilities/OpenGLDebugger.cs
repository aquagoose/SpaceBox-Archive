using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Cubic.Utilities
{
    public class OpenGLDebugger
    {
        private static DebugProc _debugProcCallback;
        private static GCHandle _debugProcHandle;
        
        public OpenGLDebugger()
        {
            _debugProcCallback = DebugCallback;
            
            _debugProcHandle = GCHandle.Alloc(_debugProcCallback);
            
            GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
        }
        
        private void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length,
            IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            if (type == DebugType.DebugTypeError)
                throw new Exception(messageString);

            Console.WriteLine(
                $"Severity: {severity.ToString().Replace("DebugSeverity", "")}, Type: {type.ToString().Replace("DebugType", "")} | {messageString}");
        }
    }
}