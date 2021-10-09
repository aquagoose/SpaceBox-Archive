using System;
using System.Drawing;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Image = OpenTK.Windowing.GraphicsLibraryFramework.Image;

namespace Cubic.Windowing
{
    public unsafe class WindowSettings
    {
        /// <summary>
        /// The size of the window.
        /// </summary>
        public Size Size { get; set; } = new Size(1280, 720);
        
        /// <summary>
        /// The title of the window.
        /// </summary>
        public string Title { get; set; } = "Cubic Game Window";

        /// <summary>
        /// If true, the window will start in fullscreen mode at the given resolution.
        /// </summary>
        public bool StartFullscreen { get; set; } = false;

        /// <summary>
        /// Set the monitor the window should open in. If none is provided, the primary monitor will be used.
        /// </summary>
        public Monitor* StartingMonitor { get; set; } = null;

        /// <summary>
        /// The number of samples to use for MSAA, if any. If none are provided, MSAA will not be used.
        /// </summary>
        public uint SampleCount { get; set; } = 0;

        /// <summary>
        /// The icon that will be used for this window.
        /// </summary>
        public WindowIcon Icon { get; set; } = default;
    }
}